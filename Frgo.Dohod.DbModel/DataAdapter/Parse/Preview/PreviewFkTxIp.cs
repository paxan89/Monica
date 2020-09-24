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
    public class PreviewFkTxIp : BasePreviewFormat, IPreviewFormat
    {
        public PreviewFkTxIp(DohodDbContext dohodDbContext) : base(dohodDbContext)
        { 
        }


        public async Task<object> PreviewFormat(object doc, ImportParam importParam)
        {
            dynamic objIP = null;
            decimal summa = 0.0M;
            s_lev lev = null;
            var reccount = 0;
            var writecount = 0;
            DateTime datemin = default(DateTime);
            DateTime datemax = default(DateTime);
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                if (obj.Marker == "IP")
                {
                    objIP = obj;
                    lev = await _dohodDbContext.IdentLevelOnSchet((string)objIP.BS);
                }
                else if (obj.Marker == "IPSTBK_E_KBK" && objIP != null)
                {
                    reccount++;
                    if (!OkSchet())
                        continue;
                    (bool ok, DateTime? date, decimal summa) rec = await DohRecord(lev, objIP, obj, importParam);
                    if (!rec.ok)
                        continue;
                    writecount++;
                    (datemin, datemax) = rec.date.GetMinMax(datemin, datemax);
                    summa += rec.summa;
                }
                else if (obj.Marker == "IPSTBK_E" && objIP != null)
                {
                    reccount++;
                    if (OkSchet())
                        continue;
                    (bool ok, DateTime? date, decimal summa) rec = await DohRecord(lev, objIP, obj, importParam);
                    if (!rec.ok)
                        continue;

                    writecount++;
                    (datemin, datemax) = rec.date.GetMinMax(datemin, datemax);
                    summa += rec.summa;
                }
            }
            return InfoPreviewData(doc, (bool)(objIP == null), reccount, writecount, summa, datemin, datemax, importParam);

            bool OkSchet() => lev != null && !string.IsNullOrWhiteSpace(lev.Inn) && !string.IsNullOrWhiteSpace(lev.Kpp);
        }



        private async Task<object> DohRecord(s_lev lev, dynamic objIP, dynamic obj, ImportParam importParam)
        {
            bool ok = false;
            DateTime? date = null;
            decimal summa = 0.0M;

            if (obj.TYPE_KBK == "20")
            {
                if (obj.Marker == "IPSTBK_E")
                    lev = await _dohodDbContext.IdentLevelOnNameBudget((string)obj.NAME_BUD);
                if (lev != null)
                {
                    var oktmopp = lev?.Oktmo ?? "";
                    var oktmo = GetOktmo(oktmopp.Substr(0, 8), importParam);
                    //  Для IP пока не фильтруем по m.o.idOkato = 0
                    //if (oktmo != null)
                    //{
                    ok = true;
                    date = objIP.DATE_OTCH;
                    summa = (decimal)obj.SUM_POST_DATE;
                    //}
                }
            }
            return (ok, date, summa);
        }




    }
}