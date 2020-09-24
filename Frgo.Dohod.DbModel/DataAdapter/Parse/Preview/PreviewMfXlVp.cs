using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.Extension.Txt;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.ImportPreviw;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.Preview
{
    public class PreviewMfXlVp : BasePreviewFormat, IPreviewFormat
    {
        public PreviewMfXlVp(DohodDbContext dohodDbContext) : base(dohodDbContext)
        { 
        }


        public async Task<object> PreviewFormat(object doc, ImportParam importParam)
        {
            (DateTime datemin, DateTime datemax) = CorrectFileName(doc, importParam);
            decimal summa = 0.0M;
            var reccount = 0;
            var writecount = 0;
            //
            dynamic objHeader = null;
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                if (obj.Marker == Constant.MarkerXlHeader)
                {
                    objHeader = obj;
                    continue;
                }
                // TB
                if (obj.KBK == null) // филтр на запись "Итого"
                    continue;

                reccount++;
                (bool ok, decimal summa) rec = await DohRecord(objHeader, obj, importParam);
                if (!rec.ok)
                    continue;

                writecount++;
                summa += rec.summa;
            }

            return InfoPreviewData(doc, (bool)(objHeader == null), reccount, writecount, summa, datemin, datemax, importParam);
        }


        private async Task<object> DohRecord(dynamic objHeader, dynamic objTb, ImportParam importParam)
        {
            bool ok = false;
            decimal summa = 0.0M;
            var codeoktmo = ((string)objTb.OKATO).Substr(0, 8);
            var oktmo = GetOktmo(codeoktmo, importParam);
            if (!string.IsNullOrWhiteSpace(objTb.KBK) && // филтр на запись "Итого"
                oktmo != null)                           // филтр на ОКТМО
            {
                ok = true;
                summa = (decimal)objTb.SUMMA;
            }
            return (ok, summa);
        }


        private (DateTime, DateTime) CorrectFileName(object doc, ImportParam importParam)
        {
            // Исходные наименования файлов не уникальны.
            // Формируется специальные уникальные наименования по содержимому файла.
            var teritory = "";
            DateTime dtheader = default(DateTime);
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