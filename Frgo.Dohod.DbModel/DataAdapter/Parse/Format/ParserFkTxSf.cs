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
    public class ParserFkTxSf : BaseParserFormat, IParserFormat
    {
        // private DohodDbContext _dohodDbContext;
        public ParserFkTxSf(DohodDbContext dohodDbContext) : base(dohodDbContext)
        {
        }
        public async Task<object> ParserFormat(object doc, ImportParam importParam)
        {
            await DeleteTheFileData(importParam);

            var dohods = new List<t_dohod>();
            bool okSF = true;
            dynamic objSF = null;
            dynamic objSFDOC = null;
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                switch (obj.Marker)
                {
                    case "SF":
                        okSF = FilterSF(obj); // это на весь файл
                        objSF = obj;
                        break;
                    
                    case "SFDOC":
                        objSFDOC = obj;
                        break;

                    case "SFST":
                        if (objSFDOC == null)
                           continue;
                        var dohod = await DohRecord(objSF, objSFDOC, obj, importParam);
                        dohod = await ChechRecord(dohod, importParam);
                        if (dohod == null)
                           continue;

                        dohods.Add(dohod);
                        break;

                    default:
                        objSFDOC = null;
                        break;
                }
                if (!okSF)
                    break;
            }

            return dohods; 
        }

        private bool FilterSF(dynamic objSF)
        {
            return objSF.KOD_DOC == "VT";
        }

        private async Task<t_dohod> DohRecord(dynamic objSF, dynamic objSFDOC, dynamic objSFST, ImportParam importParam)
        {
            t_dohod dohod = new t_dohod();
            t_dohorig dohorig = new t_dohorig();

            // вспомогательные
            dohorig.OktmoPp = objSFST.OKATO; // (поле 105)*
            var oktmo = GetOktmo(dohorig.OktmoPp.Substr(0, 8), importParam, true);
            if (oktmo == null)
                return null;
             
     
            // dohorig 
            dohorig.Oktmo = oktmo.Oktmo; // objBDPDST.ОКАТО;
            dohorig.Kbk = objSFST.KBK;
            dohorig.KbkPp = "";   // (поле 104)*
            dohorig.CodeCeli = "";
            dohorig.Inn = objSFST.INN_PAY;
            dohorig.Kpp = objSFST.KPP_PAY;
            dohorig.PlatName = "";
            dohorig.Rs = "";
            dohorig.Bic = "";
            dohorig.BankName = "";
            // если нет в справочние, добавлять не нужно 
            var org = await _dohodDbContext.IdentPlat(dohorig.Inn, dohorig.Kpp, dohorig.PlatName, dohorig.Rs, false);
 
            // dohod
            dohod.DateOper = objSF.DATE;
            dohod.Date = _dohodDbContext.GetSkipedWorkDay(dohod.DateOper ?? default(DateTime), importParam.DaysToSkip);
            dohod.NumPp = objSFST.NOM_DOC;
            dohod.DatePp = objSFST.DATE_DOC;
            dohod.NumZf = objSFDOC.NOM_SF;
            dohod.DateZf = objSFDOC.DATE_OTCH;
            dohod.OktmoId = oktmo.Sysid;
            dohod.KbkId =  (await _dohodDbContext.IdentKbk(dohorig.Kbk)).Sysid;
            dohod.CodeCeliId = null;
            dohod.PlatId = org?.Sysid;
            dohod.IsRaschet = true;
            dohod.IsSumRazb = false;
            dohod.SumItg = (decimal)objSFST.SUM_SFST;
            dohod.ImnsInn = objSFST.INN_ADB;
            dohod.ImnsKpp = objSFST.KPP_ADB;
            dohod.LsAdb = "";
            dohod.OsnPlat = "";     // (поле 106)*
            dohod.CodeVidOper = "";
            dohod.VidOper = objSFST.NAME_DOC; // "Справка органа ФК"
            dohod.NaznPlat = objSFST.OPER;   // "Уточнение види и ...."
            dohod.ImportFile = importParam.FileName;
            dohod.IsNp = dohorig.Kbk.IsNpCode();
            dohod.IsVoz = false;
            dohod.IsItg = false;
            dohod.LinkNp = null;
            dohod.LinkVoz = null;
            dohod.LinkItg = null;
            dohod.Guid = objSFDOC.GUID_FK;
            dohod.VidDoc = VidDoc.SF;
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
                // здесь только записи NP, VT, SF
                if (record.VidDoc == VidDoc.NP)
                {
                    // Записи из NP не заменяем данными SF
                    _previewdata.recsSkip++;
                    return null;
                }
                else
                {
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


        protected override async Task<t_dohod> GetExistingRecord(t_dohod dohod)
        {
            // Ищем только записи для замены
            // Если несколько записей с совпадающими параметрами, берем первую aTmp[1], далее,
            // их число будет уменьшатся, так как изменяем DOHOD.VIDDOC на VIDDOC_SF
            // Также как VT c SF, хоть номер уведомления и знаем, но использовать можно бы только среди VIDDOC_SF
            var query = await _dohodDbContext.t_dohod.AsNoTracking()
                               .Where(w => w.DateOper == dohod.DateOper
                                        && w.VidDoc == VidDoc.VT
                                        && w.DateZf == dohod.DateZf
                                        && w.NumZf == dohod.NumZf
                                        && w.KbkId == dohod.KbkId
                                        && w.SumItg == dohod.SumItg)
                               .Include(x => x.Orig)
                               .ToListAsync();
            if (query.Count == 0)
            {
                // 1.Ищем еще среди NP(приоритет NP) - 201502012 - еще общее название доукмента, напр.: "Заявка на возврат" в обоих
                query = await _dohodDbContext.t_dohod.AsNoTracking()
                               .Where(w => w.DateOper == dohod.DateOper
                                        && w.VidDoc == VidDoc.NP
                                        && w.DatePp == dohod.DatePp
                                        && w.NumPp == dohod.NumPp
                                        && w.KbkId == dohod.KbkId
                                        && w.SumItg == dohod.SumItg)
                               .Include(x => x.Orig)
                               .ToListAsync();

            }
            if (query.Count == 0)
            {
                // 2.Ищем еще среди SF - могли записать ранее из SF файла с другим наименованием. Используем GUID
                query = await _dohodDbContext.t_dohod.AsNoTracking()
                               .Where(w => w.DateOper == dohod.DateOper
                                        && w.VidDoc == VidDoc.SF
                                        && w.Guid == dohod.Guid
                                        && w.DateZf == dohod.DateZf
                                        && w.NumZf == dohod.NumZf
                                        && w.KbkId == dohod.KbkId
                                        && w.SumItg == dohod.SumItg)
                               .Include(x => x.Orig)
                               .ToListAsync();

            }
            return query.FirstOrDefault();

        }


        public override async Task<bool> AfterDataWrited(List<t_dohod> dohods, ImportParam importParam) //, bool isUvDocMarking = true)
        {
            // пытаемся определить плательщика, через уведомление
            foreach (var dohod in dohods)
            {
                var uvdet = await _dohodDbContext.IdentUved(dohod, importParam, true);
            }
            return true;
        }
    
   }
}