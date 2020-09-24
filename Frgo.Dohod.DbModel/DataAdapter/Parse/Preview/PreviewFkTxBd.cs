using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.ImportPreviw;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.Preview
{
    public class PreviewFkTxBd : BasePreviewFormat, IPreviewFormat
    {
        public PreviewFkTxBd(DohodDbContext dohodDbContext) : base(dohodDbContext)
        { 
        }


        public async Task<object> PreviewFormat(object doc, ImportParam importParam)
        {
            dynamic objBD = null;
            dynamic objBDPD = null;
            decimal summa = 0.0M;
            var reccount = 0;
            var writecount = 0;
            DateTime datemin = default(DateTime);
            DateTime datemax = default(DateTime);
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                if (obj.Marker == "BD" && FilterBD(obj))
                    objBD = obj;

                else if (obj.Marker == "BDPD" && objBD != null)
                    objBDPD = obj;

                else if (obj.Marker == "BDPDST")
                {
                    reccount++;
                    if (objBDPD == null)
                        continue;
                    (bool ok, DateTime? date, decimal summa) rec = await DohRecord(objBDPD, obj, importParam);
                    if (!rec.ok)
                        continue;

                    writecount++;
                    (datemin, datemax) = rec.date.GetMinMax(datemin, datemax);
                    summa += rec.summa;
                }

                else
                    objBDPD = null;

            }

            return InfoPreviewData(doc, (bool)(objBD == null), reccount, writecount, summa, datemin, datemax, importParam);
        }

        private bool FilterBD(dynamic objBD)
        {
            return objBD.KOD_DOC == "VT" && objBD.DATE != null;
        }


        private async Task<object> DohRecord(dynamic objBDPD, dynamic objBDPDST, ImportParam importParam)
        {
            bool ok = false;
            DateTime? date = null;
            decimal summa = 0.0M;
            var oktmo = base.GetOktmo((string)objBDPDST.ОКАТО, importParam);
            if (oktmo != null)
            {
                ok = true;
                var iSign = (objBDPDST.DIR_SUM ?? "0") == "0" ? +1.0M : -1.0M; // 0 - зачисление; 1 - списание.                                                                            //
                date = objBDPD.DATE_IN_TOFK;
                summa = (decimal)objBDPDST.SUM * iSign;
            }
            
            return (ok, date, summa);
        }




    }
}