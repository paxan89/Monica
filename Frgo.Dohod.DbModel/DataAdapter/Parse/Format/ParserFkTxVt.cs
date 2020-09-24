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
    public class ParserFkTxVt : BaseParserFormat, IParserFormat
    {
        // private DohodDbContext _dohodDbContext;
        public ParserFkTxVt(DohodDbContext dohodDbContext) : base(dohodDbContext)
        {
        }
        public async Task<object> ParserFormat(object doc, ImportParam importParam)
        {
            await DeleteTheFileData(importParam);

            var dohods = new List<t_dohod>();
            dynamic objVT = null;
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                switch (obj.Marker)
                {
                    case "VT":
                        objVT = obj;
                        break;

                    case "VTOPER":
                        var dohod = await DohRecord(objVT, obj, importParam);
                        dohod = await ChechRecord(dohod, importParam);
                        if (dohod == null)
                           continue;

                        dohods.Add(dohod);
                        break;

                    default:
                        break;
                }
            }

            return dohods; 
        }

  
        private async Task<t_dohod> DohRecord(dynamic objVT,  dynamic objVTOPER, ImportParam importParam)
        {
            t_dohod dohod = new t_dohod();
            t_dohorig dohorig = new t_dohorig();

 
            // вспомогательные
            var kodDoc = (string)objVTOPER.KOD_DOC;  // "SF" или "UF"
            if (kodDoc != "UF") // филтр - Только UF
                return null;    // c 20150216 - SF пишем только из *.SF файлов
 
            dohorig.OktmoPp = objVTOPER.OKATO; // (поле 105)*
            var oktmo = GetOktmo(dohorig.OktmoPp.Substr(0,8), importParam, true);
            if(oktmo  == null)
                return null;

            // dohorig 
            dohorig.Oktmo = oktmo.Oktmo; 
            dohorig.Kbk = objVTOPER.KBK;
            dohorig.KbkPp = "";   // (поле 104)*
            dohorig.CodeCeli = objVTOPER.ADD_KLASS;
            dohorig.Inn = "";
            dohorig.Kpp = "";
            dohorig.PlatName = "";
            dohorig.Rs = "";
            dohorig.Bic = "";
            dohorig.BankName = "";

            // dohod
            var date = (DateTime)objVT.DATE_OTCH;
            dohod.Date = _dohodDbContext.GetSkipedWorkDay(date, importParam.DaysToSkip);
            dohod.DateOper = date;
            // Номер уведомления знаем, справки нет
            dohod.NumPp = objVTOPER.NOM_DOC;
            dohod.DatePp = objVTOPER.DATE_DOC;
            dohod.NumZf = null;
            dohod.DateZf = null;
            dohod.OktmoId = oktmo.Sysid;
            dohod.KbkId =  (await _dohodDbContext.IdentKbk((string)objVTOPER.KBK)).Sysid;
            dohod.CodeCeliId = (await _dohodDbContext.IdentKceli((string)objVTOPER.ADD_KLASS))?.Sysid;
            dohod.PlatId = null;
            dohod.IsRaschet = true;
            dohod.IsSumRazb = false;
            dohod.SumItg = (decimal)objVTOPER.SUM_ZACH;
            dohod.ImnsInn = objVTOPER.INN_ADB;
            dohod.ImnsKpp = objVTOPER.KPP_ADB;
            dohod.LsAdb = "";
            dohod.OsnPlat = "";     // (поле 106)*
            dohod.CodeVidOper = ""; // Нет ??
            dohod.VidOper = "Уведомление об уточнении"; // dohod.CodeVidOper.GetVidOperName();
            dohod.NaznPlat = "";   // Назначение платежа
            dohod.ImportFile = importParam.FileName;
            dohod.IsNp = dohorig.Kbk.IsNpCode();
            dohod.IsVoz = false;
            dohod.IsItg = false;
            dohod.LinkNp = null;
            dohod.LinkVoz = null;
            dohod.LinkItg = null;
            dohod.Guid = objVTOPER.GUID;
            dohod.VidDoc = VidDoc.VT;
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
                // VT или NP
                if (record.VidDoc == VidDoc.NP)
                {
                    // Записи из NP не заменяем данными VT, Переиндетификацию уведомлений непроизводим
                    _previewdata.recsSkip++;
                    return null;
                }
                if (record.VidDoc == VidDoc.VT)
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

        protected override  async Task<t_dohod> GetExistingRecord(t_dohod dohod)
        {
            //Cо справкой не пересекается используем гуид
             var query = await _dohodDbContext.t_dohod.AsNoTracking() 
                               .Where(w => w.DateOper == dohod.DateOper
                                        && w.VidDoc == VidDoc.VT
                                        && w.Guid == dohod.Guid
                                        && w.DatePp == dohod.DatePp
                                        && w.KbkId == dohod.KbkId
                                        && w.NumPp == dohod.NumPp
                                        && w.SumItg == dohod.SumItg) 
                               .Include(x=>x.Orig)
                               .ToListAsync();
            if (query.Count==0)
            {
               //Ищем еще сред NP - но другие параметры GUID нет в NP
               query = await _dohodDbContext.t_dohod.AsNoTracking()
                               .Where(w => w.DateOper == dohod.DateOper
                                        && w.DatePp == dohod.DatePp
                                        && w.VidDoc == VidDoc.NP
                                        && w.KbkId == dohod.KbkId
                                        && w.NumPp == dohod.NumPp
                                        && w.SumItg == dohod.SumItg)
                               .Include(x => x.Orig)
                               .ToListAsync();

            }
             return query.FirstOrDefault();
        }


        public override async Task<bool> AfterDataWrited(List<t_dohod> dohods, ImportParam importParam) //, bool isUvDocMarking = true)
        {
            // пытаемся определить плательщика, через уведомление
            foreach(var dohod in dohods)//.Where(x=>x.IsNp ?? false))
            {
                var uvdet = await _dohodDbContext.IdentUved(dohod, importParam, true);
                if (uvdet != null)
                {
                    // Также для парной записи
                    var dohpair = await _dohodDbContext.t_dohod.FirstOrDefaultAsync(x => x.Guid == dohod.Guid &&
                                                                                 x.Sysid != dohod.Sysid &&
                                                                                 Math.Abs(x.SumItg) == Math.Abs(dohod.SumItg));
                    if(dohpair!=null)
                    {
                        dohpair.PlatId = dohod.PlatId;
                        dohpair.Orig.Inn = dohod.Orig.Inn; // ?? Може достаточно PlatId
                        dohpair.Orig.Kpp = dohod.Orig.Kpp;
                        _dohodDbContext.t_dohod.Update(dohpair);
                        await  _dohodDbContext.SaveChangesAsync();
                    }
                }
            }
            return true;
        }

    }
}