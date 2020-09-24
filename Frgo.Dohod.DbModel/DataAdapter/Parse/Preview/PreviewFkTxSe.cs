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
    public class PreviewFkTxSe : BasePreviewFormat, IPreviewFormat
    {
        public PreviewFkTxSe(DohodDbContext dohodDbContext) : base(dohodDbContext)
        {
        }


        public async Task<object> PreviewFormat(object doc, ImportParam importParam)
        {
            dynamic objSE = null;
            s_lev lev = null;
            decimal summa = 0.0M;
            var reccount = 0;
            var writecount = 0;
            DateTime datemin = default(DateTime);
            DateTime datemax = default(DateTime);
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                if (obj.Marker == "SE")
                {
                    objSE = obj;
                    lev = await _dohodDbContext.IdentLevelOnSchet((string)objSE.BS_UFK);
                }
                else if ((obj.Marker == "SPOST" ||
                          obj.Marker == "SVIP") && objSE != null)
                {
                    reccount++;
                    if (!OkSchet())
                        continue;
                    (bool ok, DateTime? date, decimal summa) rec = await DohRecord(lev, objSE, obj, importParam);
                    if (!rec.ok)
                        continue;
                    writecount++;
                    (datemin, datemax) = rec.date.GetMinMax(datemin, datemax);
                    summa += rec.summa;
                }
                 
            }
            return InfoPreviewData(doc, (bool)(objSE == null), reccount, writecount, summa, datemin, datemax, importParam);

            bool OkSchet() => lev != null && !string.IsNullOrWhiteSpace(lev.Inn) && !string.IsNullOrWhiteSpace(lev.Kpp);
        }



        private async Task<object> DohRecord(s_lev lev, dynamic objSE, dynamic obj, ImportParam importParam)
        {
            bool ok = false;
            DateTime? date = null;
            decimal summa = 0.0M;

            if (obj.TYPE_KBK == "20" &&
                lev != null)
            {
                var oktmopp = lev?.Oktmo ?? "";
                var oktmo = GetOktmo(oktmopp.Substr(0, 8), importParam);
                //  Для SE  не фильтруем по m.o.idOkato = 0 он уже из lev
                //if (oktmo != null)
                //{
                ok = true;
                date = objSE.DATE_SP;
                if(obj.Marker == "SPOST")
                   summa = (decimal)obj.SUM_POST;
                else if(obj.Marker== "SVIP")
                   summa = (decimal)obj.SUM_VIP;
                //}
            }
            return (ok, date, summa);
        }




    }
}