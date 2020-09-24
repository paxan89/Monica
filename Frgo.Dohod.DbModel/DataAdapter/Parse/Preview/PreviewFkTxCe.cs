using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.Extension.Txt;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Dictionary;
using Frgo.Dohod.DbModel.ModelCrm.ImportPreviw;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.Preview
{
    public class PreviewFkTxCe : BasePreviewFormat, IPreviewFormat
    {
        public PreviewFkTxCe(DohodDbContext dohodDbContext) : base(dohodDbContext)
        {
        }


        public async Task<object> PreviewFormat(object doc, ImportParam importParam)
        {
            dynamic objCE = null;
            decimal summa = 0.0M;
            s_lev lev = null;
            var reccount = 0;
            var writecount = 0;
            DateTime datemin = default(DateTime);
            DateTime datemax = default(DateTime);
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                if (obj.Marker == "CE")
                {
                    objCE = obj;
                    lev = await _dohodDbContext.IdentLevelOnSchet((string)objCE.BS);
                }
                else if (obj.Marker == "CED" && objCE != null)
                {
                    reccount++;
                    if (!OkSchet())
                        continue;
                    (bool ok, DateTime? date, decimal summa) rec = await DohRecord(lev, objCE, obj, importParam);
                    if (!rec.ok)
                        continue;
                    writecount++;
                    (datemin, datemax) = rec.date.GetMinMax(datemin, datemax);
                    summa += rec.summa;
                }
                 
            }
            return InfoPreviewData(doc, (bool)(objCE == null), reccount, writecount, summa, datemin, datemax, importParam);

            bool OkSchet() => lev != null && !string.IsNullOrWhiteSpace(lev.Inn) && !string.IsNullOrWhiteSpace(lev.Kpp);
        }



        private async Task<object> DohRecord(s_lev lev, dynamic objCE, dynamic objCED, ImportParam importParam)
        {
            bool ok = false;
            DateTime? date = null;
            decimal summa = 0.0M;

            if (objCED.TYPE_KBK == "20" &&
                lev != null)
            {
                var oktmopp = lev?.Oktmo ?? "";
                var oktmo = GetOktmo(oktmopp.Substr(0, 8), importParam);
                //  Для CE  не фильтруем по m.o.idOkato = 0 он уже из lev
                //if (oktmo != null)
                //{
                ok = true;
                date = objCE.DATE_OTCH;
                var sumPost = (decimal)objCED.SUM_POST;
                var sumVib = (decimal)objCED.SUM_VIP;
                summa = sumPost - sumVib;
                //}
            }
            return (ok, date, summa);
        }




    }
}