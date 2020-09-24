using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.Manager.Parse;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Data;
using Frgo.Dohod.DbModel.ModelCrm.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.Format
{
    public class ParserFkTxBd : BaseParserFormat, IParserFormat
    {
        // private DohodDbContext _dohodDbContext;
        public ParserFkTxBd(DohodDbContext dohodDbContext) : base(dohodDbContext)
        {
        }
        public async Task<object> ParserFormat(object doc, ImportParam importParam)
        {
            await DeleteTheFileData(importParam);

            var dohods = new List<t_dohod>();
            dynamic objBD = null;
            dynamic objBDPD = null;
            s_org org = null;
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                switch (obj.Marker)
                {
                    case "BD":
                        if (!FilterBD(obj))
                            continue;
                        objBD = obj;
                        break;

                    case "BDPD":
                        if (objBD == null)
                           continue;
                        org = await _dohodDbContext.IdentPlat((string)obj.INN_PAY, (string)obj.KPP_PAY, (string)obj.CNAME_PAY, (string)obj.BS_PAY);
                        if (org == null)
                            continue;
                        objBDPD = obj;
                        break;

                    case "BDPDST":
                        if (objBDPD == null)
                           continue;
                        var dohod = await DohRecord(org, objBD, objBDPD, obj, importParam);
                        dohod = await ChechRecord(dohod, importParam);
                        if (dohod == null)
                           continue;

                        dohods.Add(dohod);
                        break;

                    default:
                        objBDPD = null;
                        break;
                }
            }

            return dohods; 
        }

        private bool FilterBD(dynamic objBD)
        {
            return objBD.KOD_DOC == "VT"&& objBD.DATE != null;
        }

        private async Task<t_dohod> DohRecord(s_org org, dynamic objBD, dynamic objBDPD, dynamic objBDPDST, ImportParam importParam)
        {
            t_dohod dohod = new t_dohod();
            t_dohorig dohorig = new t_dohorig();

            // вспомогательные
            var oktmo = GetOktmo((string)objBDPDST.ОКАТО, importParam);
            if(oktmo  == null)
                return null;
            var iSign = (objBDPDST.DIR_SUM ?? "0")=="0" ? +1.0M : -1.0M ; // 0 - зачисление; 1 - списание.

            // dohorig 
            dohorig.Oktmo = oktmo.Oktmo; // objBDPDST.ОКАТО;
            dohorig.OktmoPp = objBDPD.OKATO; // (поле 105)*
            dohorig.Kbk = objBDPDST.KBK;
            dohorig.KbkPp = objBDPD.KBK;   // (поле 104)*
            dohorig.CodeCeli = objBDPDST.ADD_KLASS;
            dohorig.Inn = objBDPD.INN_PAY;
            dohorig.Kpp = objBDPD.KPP_PAY;
            dohorig.PlatName = objBDPD.CNAME_PAY;
            dohorig.Rs = objBDPD.BS_PAY;
            dohorig.Bic = objBDPD.BIC_PAY;
            dohorig.BankName = objBDPD.NAME_BIC_PAY;

            // dohod
            dohod.Date = _dohodDbContext.GetSkipedWorkDay((DateTime)objBDPD.DATE_IN_TOFK, importParam.DaysToSkip);
            dohod.DateOper = objBDPD.DATE_IN_TOFK;
            dohod.NumPp = objBDPD.NUM_PP;
            dohod.DatePp = objBDPD.DATE_PP;
            dohod.NumZf = null;
            dohod.DateZf = null;
            dohod.OktmoId = oktmo.Sysid;
            dohod.KbkId =  (await _dohodDbContext.IdentKbk((string)objBDPDST.KBK)).Sysid;
            dohod.CodeCeliId = (await _dohodDbContext.IdentKceli((string)objBDPDST.ADD_KLASS))?.Sysid;
            dohod.PlatId = org.Sysid;
            dohod.IsRaschet = true;
            dohod.IsSumRazb = false;
            dohod.SumItg = (decimal)objBDPDST.SUM * iSign;
            dohod.ImnsInn = objBDPD.INN_RCP;
            dohod.ImnsKpp = objBDPD.KPP_RCP;
            dohod.LsAdb = objBD.LS;
            dohod.OsnPlat = objBDPD.OSNPLAT;     // (поле 106)*
            dohod.CodeVidOper = objBDPD.VID_OPER;
            dohod.VidOper = dohod.CodeVidOper.GetVidOperName();
            dohod.NaznPlat = objBDPD.PURPOSE;   // Назначение платежа
            dohod.ImportFile = importParam.FileName;
            dohod.IsNp = dohorig.Kbk.IsNpCode();
            dohod.IsVoz = false;
            dohod.IsItg = false;
            dohod.LinkNp = null;
            dohod.LinkVoz = null;
            dohod.LinkItg = null;
            dohod.Guid = objBDPD.GUID;
            dohod.VidDoc = VidDoc.BD;
            //
            dohod.Orig = dohorig;
            await _dohodDbContext.RazbivkaItogSum(dohod);

            return dohod;
        }



        protected override  async Task<t_dohod> GetExistingRecord(t_dohod dohod)
        {
            // Для "BD"
            // Ищем только записи для замены среди записей XL, NP типов
            // Если несколко записей с совпадающими параметрами берем первую, далее
            // их число будет уменьшаться, т.к. изменяем VidDoc на BD
            var query = await _dohodDbContext.t_dohod.AsNoTracking() 
                               .Where(w => w.DateOper == dohod.DateOper
                                        && w.VidDoc == VidDoc.NP
                                        && w.PlatId == dohod.PlatId
                                        && w.KbkId == dohod.KbkId
                                        && w.NumPp == dohod.NumPp
                                        && w.SumItg == dohod.SumItg) 
                               .Include(x=>x.Orig)
                               .ToListAsync();
            if (query.Count==0)
            {
                query = await _dohodDbContext.t_dohod.AsNoTracking()
                               .Where(w => w.DatePp == dohod.DatePp
                                        && w.VidDoc == VidDoc.XL
                                        && w.PlatId == dohod.PlatId
                                        && w.KbkId == dohod.KbkId
                                        && w.OktmoId == dohod.OktmoId
                                        && w.NumPp == dohod.NumPp
                                        && w.SumItg == dohod.SumItg
                                        && (w.IsRaschet ?? false) )
                               .Include(x => x.Orig)
                               .ToListAsync();

            }
            if (query.Count==0)
            {
                // Ищем еще среди BD - могли записать раннее из BD файла с другим наименоанием
                // Используеться GUID
                query = await _dohodDbContext.t_dohod.AsNoTracking()
                               .Where(w => w.DateOper == dohod.DateOper
                                        && w.VidDoc == VidDoc.BD
                                        && w.PlatId == dohod.PlatId
                                        && w.KbkId == dohod.KbkId
                                        && w.NumPp == dohod.NumPp
                                        && w.SumItg == dohod.SumItg
                                        && w.Guid == dohod.Guid )
                               .Include(x => x.Orig)
                               .ToListAsync();

            }
            return query.FirstOrDefault();
        }
   }
}