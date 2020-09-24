using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.Extension.Txt;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.ImportPreviw;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.Preview
{
    public class PreviewFkTxZf : BasePreviewFormat, IPreviewFormat
    {
        public PreviewFkTxZf(DohodDbContext dohodDbContext) : base(dohodDbContext)
        { 
        }


        public async Task<object> PreviewFormat(object doc, ImportParam importParam)
        {
            decimal summa = 0.0M;
            var reccount = 0;
            var writecount = 0;
            DateTime datemin = default(DateTime);
            DateTime datemax = default(DateTime);
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                if (obj.Marker == "ZF")
                {
                    reccount++;
                    (bool ok, DateTime? date, decimal summa) rec = DohRecord(obj, importParam);
                    if (!rec.ok)
                        continue;

                    writecount++;
                    (datemin, datemax) = rec.date.GetMinMax(datemin, datemax);
                    summa += rec.summa;
                }

            }

            return InfoPreviewData(doc, reccount==0, reccount, writecount, summa, datemin, datemax, importParam);
        }

  

        private object DohRecord(dynamic objZF, ImportParam importParam)
        {
            bool ok = false;
            DateTime? date = null;
            decimal summa = 0.0M;
            ok = true;
            summa = (decimal)objZF.SUM_PP;
            date = objZF.DATE_ZF;

            return (ok, date, summa);
        }




    }
}