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
    public class PreviewFkTxVt : BasePreviewFormat, IPreviewFormat
    {
        public PreviewFkTxVt(DohodDbContext dohodDbContext) : base(dohodDbContext)
        { 
        }


        public async Task<object> PreviewFormat(object doc, ImportParam importParam)
        {
            dynamic objVT = null;
            decimal summa = 0.0M;
            var reccount = 0;
            var writecount = 0;
            DateTime datemin = default(DateTime);
            DateTime datemax = default(DateTime);
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                if (obj.Marker == "VT")
                {
                    objVT = obj;
                    datemin = obj.DATE_OTCH;
                    datemax = obj.DATE_OTCH;
                }

                else if (obj.Marker == "VTOPER")
                {
                    reccount++;
                    (bool ok,  decimal summa) rec = await DohRecord(obj, importParam);
                    if (!rec.ok)
                        continue;

                    writecount++;
                    summa += rec.summa;
                }
            }

            return InfoPreviewData(doc, (bool)(objVT == null), reccount, writecount, summa, datemin, datemax, importParam);
        }


        private async Task<object> DohRecord(dynamic objVTOPER, ImportParam importParam)
        {
            bool ok = false;
            decimal summa = 0.0M;

            // вспомогательные
            var kodDoc = (string)objVTOPER.KOD_DOC;  // "UF"
            if (kodDoc == "UF") 
            {
                // филтр - Только UF
                var oktmo = base.GetOktmo(((string)objVTOPER.OKATO).Substr(0, 8), importParam);
                if (oktmo != null)
                {
                    ok = true;
                    summa = (decimal)objVTOPER.SUM_ZACH;
                }
            }
            return (ok, summa);
        }




    }
}