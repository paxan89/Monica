using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.Extension.Txt;
using Frgo.Dohod.DbModel.Manager.Parse;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Data;
using Frgo.Dohod.DbModel.ModelCrm.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.Format
{
    public class ParserFkTxNp : BaseParserFormat, IParserFormat
    {
        // private DohodDbContext _dohodDbContext;
        public ParserFkTxNp(DohodDbContext dohodDbContext) : base(dohodDbContext)
        {
        }
        public async Task<object> ParserFormat(object doc, ImportParam importParam)
        {
            await DeleteTheFileData(importParam);

            var dohods = new List<t_dohod>();
            dynamic objNP = null;
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                switch (obj.Marker)
                {
                    case "NP":
                        objNP = obj;
                        break;

                    case "NPST":
                        if (objNP == null)
                           continue;
                        var dohod = await DohRecord(objNP, obj, importParam);
                        dohod = await ChechRecord(dohod, importParam);
                        if (dohod == null)
                           continue;

                        dohods.Add(dohod);
                        break;

                }
            }

            return dohods; 
        }

        private async Task<t_dohod> DohRecord(dynamic objNP, dynamic objNPST, ImportParam importParam)
        {
            t_dohod dohod = new t_dohod();
            t_dohorig dohorig = new t_dohorig();
            var dateOtch = (DateTime)objNP.DATE_OTCH;
            var dateEnd = (DateTime)objNP.DATE_END_PER;
            if (dateOtch != dateEnd)
                dateOtch = objNPST.DATE_DOC_VB;
            if (dateOtch == null)
                return null; // Пока пропускаем, могли писать бы на какую то дату MIN(MAX(m.dDateBeg, m.o.dDatePP + 2), m.dDateEnd) например

            if (objNPST.TYPE_KBK != "20")
                return null;

            string  oktmopp = objNPST.OKATO;
            // Область обязательно, даже если не прописана в ca_okato(из за акцизов)
            var isOblast = oktmopp.StartsWith("45000000") || oktmopp.StartsWith("46000000");
            var oktmo = GetOktmo(oktmopp.Substr(0,8), importParam);
            if(oktmo  == null && !isOblast)
                return null;

 
            // dohorig 
            dohorig.Oktmo = oktmo?.Oktmo ?? oktmopp.Substr(0,8); // objBDPDST.ОКАТО;
            dohorig.OktmoPp = oktmopp; // (поле 105)*
            dohorig.Kbk = objNPST.KBK;
            dohorig.KbkPp = dohorig.Kbk;   // (поле 104)*
            dohorig.CodeCeli = "";
            dohorig.Inn = objNPST.INN_PAY;
            dohorig.Kpp = objNPST.KPP_PAY;
            dohorig.PlatName = objNPST.NAME_PAY;
            dohorig.Rs = "";
            dohorig.Bic = "";
            dohorig.BankName = "";

            // dohod
            dohod.DateOper = objNP.DATE_END_PER;
            dohod.Date = _dohodDbContext.GetSkipedWorkDay((DateTime)objNP.DATE_END_PER, importParam.DaysToSkip);
            dohod.NumPp = objNPST.NOM_DOC;
            dohod.DatePp = objNPST.DATE_DOC;
            dohod.NumZf = null;
            dohod.DateZf = null;
            dohod.OktmoId = oktmo.Sysid;
            dohod.KbkId =  (await _dohodDbContext.IdentKbk(dohorig.Kbk)).Sysid;
            dohod.CodeCeliId = null;
            dohod.PlatId = (await _dohodDbContext.IdentPlat(dohorig)).Sysid;
            dohod.IsRaschet = true;
            dohod.IsSumRazb = false;
            dohod.SumItg = (decimal)objNPST.SUM;
            dohod.ImnsInn = objNPST.INN_ADB;
            dohod.ImnsKpp = objNPST.KPP_ADB;
            dohod.LsAdb = "";
            dohod.OsnPlat = "";     // (поле 106)*
            dohod.VidOper = objNPST.NAME_DOC;
            dohod.CodeVidOper = dohod.VidOper == "Платежное поручение" ? "01" : "";
            dohod.NaznPlat = objNPST.NOTE;   // Назначение платежа !!! в Фох считывали с какого то файла отдельно
            dohod.ImportFile = importParam.FileName;
            dohod.IsNp = dohorig.Kbk.IsNpCode();
            dohod.IsVoz = false;
            dohod.IsItg = false;
            dohod.LinkNp = null;
            dohod.LinkVoz = null;
            dohod.LinkItg = null;
            dohod.Guid = "";
            dohod.VidDoc = VidDoc.NP;
            //
            dohod.Orig = dohorig;
            await _dohodDbContext.RazbivkaItogSum(dohod);

            return dohod;
        }

        protected override async Task<t_dohod> ChechRecord(t_dohod dohod, ImportParam importParam)
        {
            if (dohod == null)
                return null;

            var record = await GetExistingRecord(dohod);
            if (record != null)
            {
                // 
                if (record.VidDoc == VidDoc.BD)
                {
                    // Пропускаем BD
                    _previewdata.recsSkip++;
                    return null;
                }
                else
                {
                    // Запись из XL, VT, NP, SF -Обновляем 
                    _previewdata.recsUpd++;
                    dohod.Sysid = record.Sysid;
                    dohod.Orig.Sysid = record.Orig.Sysid;
                    dohod.Orig.DohodId = record.Sysid;
                    return dohod;
                }
            }

            _previewdata.recsAdd++;
            return dohod;
        }

        protected override  async Task<t_dohod> GetExistingRecord(t_dohod dohod)
        {
            // 1.Ищем только записи для замены среди XL записей,
            // 2. пропуска среди NP записей(добавили VIDDOC_VT 20140507)
            // Если несколько записей с совпадающими параметрами, берем первую aTmp[1], далее,
            // их число будет уменьшатся, так как изменяем DOHOD.VIDDOC на VIDDOC_BD
            var query = await _dohodDbContext.t_dohod.AsNoTracking() 
                               .Where(w => w.DateOper == dohod.DateOper
                                        && w.VidDoc == VidDoc.BD
                                        && w.PlatId == dohod.PlatId
                                        && w.KbkId == dohod.KbkId
                                        && w.NumPp == dohod.NumPp
                                        && w.SumItg == dohod.SumItg) 
                               .Include(x=>x.Orig)
                               .ToListAsync();
            if (query.Count==0)
            {
                query = await _dohodDbContext.t_dohod.AsNoTracking()
                               .Where(w => w.DatePp == dohod.DatePp
                                        && w.VidDoc == VidDoc.XL  
                                        && w.PlatId == dohod.PlatId
                                        && w.KbkId == dohod.KbkId
                                        && w.OktmoId == dohod.OktmoId
                                        && w.NumPp == dohod.NumPp
                                        && w.SumItg == dohod.SumItg
                                        && (w.IsRaschet ?? false) )
                               .Include(x => x.Orig)
                               .ToListAsync();

            }
            if (query.Count==0)
            {
                // Ищем еще сред VT - но другие параметры ИНН - КПП нет в VT
                // Ищем еще среди SV приоритет NP - 201502012
                query = await _dohodDbContext.t_dohod.AsNoTracking()
                               .Where(w => w.DateOper == dohod.DateOper
                                        && (w.VidDoc == VidDoc.VT  || w.VidDoc == VidDoc.SF)
                                        && w.DatePp == dohod.DatePp
                                        && w.KbkId == dohod.KbkId
                                        && w.NumPp == dohod.NumPp
                                        && w.SumItg == dohod.SumItg)                                       
                               .Include(x => x.Orig)
                               .ToListAsync();

            }
             if (query.Count==0)
            {
                // Ищем еще сред NT - в файлах раиона и поселения бывает одинакоые записи 
                // - а район имортирует файлы поселений тоже(Одинцово)
                query = await _dohodDbContext.t_dohod.AsNoTracking()
                               .Where(w => w.DateOper == dohod.DateOper
                                        && w.VidDoc == VidDoc.NP
                                        && w.DatePp == dohod.DatePp
                                        && w.PlatId == dohod.PlatId
                                        && w.KbkId == dohod.KbkId
                                        && w.SumItg == dohod.SumItg 
                                        && !(w.ImportFile == dohod.ImportFile)) // для решения проблемы, если несколько одинаковых записей
                               .Include(x => x.Orig)
                               .ToListAsync();

            }
           return query.FirstOrDefault();
        }
   }
}