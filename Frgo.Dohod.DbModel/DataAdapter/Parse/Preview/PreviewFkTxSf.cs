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
    public class PreviewFkTxSf : BasePreviewFormat, IPreviewFormat
    {
        public PreviewFkTxSf(DohodDbContext dohodDbContext) : base(dohodDbContext)
        { 
        }


        public async Task<object> PreviewFormat(object doc, ImportParam importParam)
        {
            var reccount = 0;
            var writecount = 0;
            decimal summa = 0.0M;
            DateTime datemin = default(DateTime);
            DateTime datemax = default(DateTime);
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                if (obj.Marker == "SF")
                { 
                    if(!FilterSF(obj))
                       break;
                    datemin = obj.DATE;
                    datemax = obj.DATE;
                }
 
                else if (obj.Marker == "SFST")
                {
                    reccount++;
                    (bool ok, decimal summa) rec = await DohRecord(obj, importParam);
                    if (!rec.ok)
                        continue;

                    writecount++;
                    summa += rec.summa;
                }

            }

            return InfoPreviewData(doc, datemin == default(DateTime), reccount, writecount, summa, datemin, datemax, importParam);
        }

        private bool FilterSF(dynamic objSF)
        {
            return objSF.KOD_DOC == "VT";
        }


        private async Task<object> DohRecord(dynamic objSFST, ImportParam importParam)
        {
            bool ok = false;
            DateTime? date = null;
            decimal summa = 0.0M;
           string oktmopp = objSFST.OKATO; 
            var oktmo = base.GetOktmo(oktmopp.Substr(0, 8), importParam);
            if (oktmo != null)
            {
                ok = true;
                summa = (decimal)objSFST.SUM_SFST;
            }
            
            return (ok, summa);
        }




    }
}