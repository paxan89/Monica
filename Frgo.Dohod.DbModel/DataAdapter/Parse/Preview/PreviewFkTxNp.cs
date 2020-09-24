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
    public class PreviewFkTxNp : BasePreviewFormat, IPreviewFormat
    {
        public PreviewFkTxNp(DohodDbContext dohodDbContext) : base(dohodDbContext)
        { 
        }


        public async Task<object> PreviewFormat(object doc, ImportParam importParam)
        {
             dynamic objNP = null;
            decimal summa = 0.0M;
            var reccount = 0;
            var writecount = 0;
            DateTime datemin = default(DateTime);
            DateTime datemax = default(DateTime);
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                if (obj.Marker == "NP")
                    objNP = obj;

                else if (obj.Marker == "NPST")
                {
                    reccount++;
                    if (objNP == null)
                        continue;
                    (bool ok, DateTime? date, decimal summa) rec = await DohRecord(objNP, obj, importParam);
                    if (!rec.ok)
                        continue;

                    writecount++;
                    (datemin, datemax) = rec.date.GetMinMax(datemin, datemax);
                    summa += rec.summa;
                }
            }

            return InfoPreviewData(doc, (bool)(objNP == null), reccount, writecount, summa, datemin, datemax, importParam);
        }


        private async Task<object> DohRecord(dynamic objNP, dynamic objNPST, ImportParam importParam)
        {
            bool ok = false;
            DateTime? date = null;
            decimal summa = 0.0M;

            var dateOtch = (DateTime)objNP.DATE_OTCH;
            var dateEnd = (DateTime)objNP.DATE_END_PER;
            if (dateOtch != dateEnd)
                dateOtch = objNPST.DATE_DOC_VB;
            if (dateOtch != null && objNPST.TYPE_KBK == "20")
            {
                // пропускаем, могли писать бы на какую то дату MIN(MAX(m.dDateBeg, m.o.dDatePP + 2), m.dDateEnd) например
                string oktmopp = objNPST.OKATO;
                // Область обязательно, даже если не прописана в ca_okato(из за акцизов)
                var isOblast = oktmopp.StartsWith("45000000") || oktmopp.StartsWith("46000000");
                var oktmo = GetOktmo(oktmopp.Substr(0, 8), importParam);
                if (oktmo != null || isOblast)
                {
                    // Пишем
                    ok = true;
                    date = dateOtch;
                    summa = (decimal)objNPST.SUM;
                }
            }
            return (ok, date, summa);
        }




    }
}