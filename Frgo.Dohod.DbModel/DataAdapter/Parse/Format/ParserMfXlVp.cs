using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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
    public class ParserMfXlVp : BaseParserFormat, IParserFormat
    {
        public ParserMfXlVp(DohodDbContext dohodDbContext) : base(dohodDbContext)
        {
        }

        public async Task<object> ParserFormat(object doc, ImportParam importParam)
        {
            (DateTime dtMin, DateTime dtmax) = CorrectFileName(doc, importParam);
            await DeleteTheFileData(importParam);
            //
            var dohods = new List<t_dohod>();
            dynamic objHeader = null;
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                if (obj.Marker == Constant.MarkerXlHeader)
                {
                    objHeader = obj;
                    continue;
                }
                // TB
                var org = await _dohodDbContext.IdentPlat((string)obj.INN_PAY, (string)obj.KPP_PAY, (string)obj.CNAME_PAY, "");
                var dohod = await DohRecord(org, objHeader, obj, importParam, dtMin, dtmax);
                dohod = await ChechRecord(dohod, importParam);
                if (dohod != null)
                    dohods.Add(dohod);
            }

            return dohods;
        }

        private async Task<t_dohod> DohRecord(s_org org, dynamic objHeader, dynamic objTb, ImportParam importParam, DateTime dtmin, DateTime dtmax)
        {
            if (org == null)
                return null;
            if (string.IsNullOrWhiteSpace(objTb.KBK)) // филтр на запись "Итого"
                return null;

            t_dohod dohod = new t_dohod();
            t_dohorig dohorig = new t_dohorig();

            //// вспомогательные
            var codeoktmo = ((string)objTb.OKATO).Substr(0, 8);
            var oktmo = GetOktmo(codeoktmo, importParam);
            if (oktmo == null)
                return null;

            DateTime operDate;
            DateTime skipedDate;
            if (objTb.DATE_IN_TOFK == null && dtmin != dtmax)
            { 
                // в файле разные даты
                operDate = dtmax;      // на последнюю дату
                skipedDate = dtmax;    // и без смещения
            }
            else
            {
                // файл за один день - общие правила
                operDate = objTb.DATE_IN_TOFK == null ? dtmax : (DateTime)objTb.DATE_IN_TOFK; 
                skipedDate = _dohodDbContext.GetSkipedWorkDay(operDate, importParam.DaysToSkip);
            }


            //// dohorig 
            dohorig.Oktmo = codeoktmo;
            dohorig.OktmoPp = objTb.OKATO; // пишем необрезанный 
            dohorig.Kbk = objTb.KBK;
            dohorig.KbkPp = "";   // (поле 104)*
            dohorig.CodeCeli = "";
            dohorig.Inn = objTb.INN_PAY;
            dohorig.Kpp = objTb.KPP_PAY;
            dohorig.PlatName = objTb.CNAME_PAY;
            dohorig.Rs = "";
            dohorig.Bic = "";
            dohorig.BankName = "";

            //// dohod
            dohod.Date = skipedDate;
            dohod.DateOper = operDate;
            dohod.NumPp = objTb.NOM_PP;
            dohod.DatePp = objTb.DATE_PP;
            dohod.NumZf = null;
            dohod.DateZf = null;
            dohod.OktmoId = oktmo.Sysid;
            dohod.KbkId = (await _dohodDbContext.IdentKbk((string)objTb.KBK)).Sysid;
            dohod.CodeCeliId = null;
            dohod.PlatId = org.Sysid;
            dohod.IsRaschet = true;
            dohod.IsSumRazb = false;
            dohod.SumItg = (decimal)objTb.SUMMA; // Сумма со сзаком
            dohod.ImnsInn = objTb.INN_RCP;
            dohod.ImnsKpp = objTb.KPP_RCP;
            dohod.LsAdb = "";
            dohod.OsnPlat = "";                 // (поле 106)*
            dohod.CodeVidOper = objTb.VID_OPERNAME == "Платежное поручение" ? "01" : "";
            dohod.VidOper = objTb.VID_OPERNAME; // dohod.CodeVidOper.GetVidOperName();
            dohod.NaznPlat = objTb.PURPOSE;     // Назначение платежа
            dohod.ImportFile = importParam.FileName;
            dohod.IsNp = dohorig.Kbk.IsNpCode() && dohod.SumItg >= 0.0M;
            dohod.IsVoz = false;
            dohod.IsItg = false;
            dohod.LinkNp = null;
            dohod.LinkVoz = null;
            dohod.LinkItg = null;
            dohod.Guid = "";
            dohod.VidDoc = VidDoc.XL;
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
                _previewdata.recsSkip++;
                return null;
            }

            _previewdata.recsAdd++;
            return dohod;
        }



        protected override async Task<t_dohod> GetExistingRecord(t_dohod dohod)
        {
            // Для "XL"
            // Всегда добавляем(кроме записей импортированых из BD, NP которые пропускаем)
            // Здесь возникает проблема если несколько записей с совпадающими параметрами,
            // Возможно_ 1) tally > 1      и / или
            // 2) > 1 записи в файле xl
            // Считаем, что их всех пропускаем, для _tally > 0! Можно бы запоминать, чтобы пропускалась не более 1 раза
            var lst = await _dohodDbContext.t_dohod.AsNoTracking()
                               .Where(w => w.DatePp == dohod.DatePp
                                        && (w.VidDoc == VidDoc.BD || w.VidDoc == VidDoc.NP)
                                        && w.PlatId == dohod.PlatId
                                        && w.KbkId == dohod.KbkId
                                        && w.OktmoId == dohod.OktmoId
                                        && w.NumPp == dohod.NumPp
                                        && w.SumItg == dohod.SumItg
                                        && (w.IsRaschet ?? false))
                               .Include(x => x.Orig)
                               .ToListAsync();
             return lst.FirstOrDefault();
        }


        private (DateTime, DateTime) CorrectFileName(object doc, ImportParam importParam)
        {
            // Исходные наименования файлов не уникальны.
            // Формируется специальные уникальные наименования по содержимому файла.
            var teritory = "";
            DateTime dtheader= default(DateTime);
            DateTime dtmin = default(DateTime);
            DateTime dtmax = default(DateTime);
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                if (obj.Marker == Constant.MarkerXlHeader)
                {
                    dtheader = obj.DATEVYB;
                    teritory = obj.TERRITORY;
                    continue;
                }
                // TB
                (dtmin, dtmax) = ((DateTime?)obj.DATE_IN_TOFK).GetMinMax(dtmin, dtmax);
                if (dtmin == default(DateTime))
                    (dtmin, dtmax) = (dtheader, dtheader);
            }
            importParam.FileName = $"{teritory}-{dtmin:MMdd}-{dtmax:dd}{Path.GetExtension(importParam.FileName)}";
            return (dtmin, dtmax);
        }
    }
}
