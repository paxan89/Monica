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
    public class ParserFkTxZf : BaseParserFormat, IParserFormat
    {
        public ParserFkTxZf(DohodDbContext dohodDbContext) : base(dohodDbContext)
        {
        }
        public async Task<object> ParserFormat(object doc, ImportParam importParam)
        {
            await DeleteTheFileData(importParam);

            var dohods = new List<t_dohod>();
            dynamic objZF = null;
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                switch (obj.Marker)
                {
                    case "ZF":
                        objZF = obj;
                        break;

                    case "ZF_PP":
                        if (objZF == null)
                            continue;
                        var dohod = await DohRecord(objZF, obj, importParam);
                        dohod = await ChechRecord(dohod, importParam);
                        if (dohod == null)
                            continue;

                        dohods.Add(dohod);
                        break;

                    default:
                        objZF = null;
                        break;
                }
            }

            return dohods;
        }


        private async Task<t_dohod> DohRecord(dynamic objZF, dynamic objZFPP, ImportParam importParam)
        {
            t_dohod dohod = new t_dohod();
            t_dohorig dohorig = new t_dohorig();

            // вспомогательные
            var oktmo = GetOktmo((string)objZF.OKATO_PP, importParam);
            //if (oktmo == null)
            //    return null;
 
            // dohorig 
            dohorig.Oktmo = objZF.OKATO_PP;  
            dohorig.OktmoPp = objZFPP.OKATO; // (поле 105)*
            dohorig.Kbk = objZF.KBK_PP;
            dohorig.KbkPp = objZFPP.KBK;   // (поле 104)*
            dohorig.CodeCeli = objZF.ADD_KLASS;
            dohorig.Inn = objZF.INN_PL;
            dohorig.Kpp = objZF.KPP_PL;
            dohorig.PlatName = objZF.CNAME_PL;
            dohorig.Rs = objZFPP.BS_PAY;
            dohorig.Bic = objZFPP.BIC_PAY;
            dohorig.BankName = objZFPP.NAME_BIC_PAY;

            // dohod
            dohod.DateOper = objZF.DATE_ZF;
            dohod.Date = _dohodDbContext.GetSkipedWorkDay(dohod.DateOper ?? default(DateTime), importParam.DaysToSkip);
            dohod.NumPp = objZF.NUM_PP;
            dohod.DatePp = objZF.DATE_PP;
            dohod.NumZf = objZF.NOM_ZF;
            dohod.DateZf = dohod.DateOper;
            dohod.OktmoId = oktmo?.Sysid;
            dohod.KbkId = (await _dohodDbContext.IdentKbk(dohorig.Kbk))?.Sysid;
            dohod.CodeCeliId = (await _dohodDbContext.IdentKceli(dohorig.CodeCeli))?.Sysid;
            dohod.PlatId = (await _dohodDbContext.IdentPlat(dohorig)).Sysid;

            dohod.IsRaschet = false; // всегда не показывается
            dohod.IsSumRazb = false;
            dohod.SumItg = (decimal)objZF.SUM_PP;
            dohod.ImnsInn = objZFPP.INN_RCP;
            dohod.ImnsKpp = objZFPP.KPP_RCP;
            dohod.LsAdb = "";
            dohod.OsnPlat = objZFPP.OSNPLAT;     // (поле 106)*
            dohod.CodeVidOper = objZFPP.VID_OPER;
            dohod.VidOper = objZF.NAME_PP; // dohod.CodeVidOper.GetVidOperName();
            dohod.NaznPlat = objZF.PURPOSE_PP;   // Назначение платежа
            dohod.ImportFile = importParam.FileName;
            dohod.IsNp = dohorig.Kbk.IsNpCode();
            dohod.IsVoz = false;
            dohod.IsItg = false;
            dohod.LinkNp = null;
            dohod.LinkVoz = null;
            dohod.LinkItg = null;
            dohod.Guid = objZFPP.GUID;
            dohod.VidDoc = VidDoc.ZF;
            //
            dohod.Orig = dohorig;
            await _dohodDbContext.RazbivkaItogSum(dohod);

            return dohod;
        }

        // ChechRecord - base

        protected override async Task<t_dohod> GetExistingRecord(t_dohod dohod)
        {
            // с другими типами не пересекается
            var query = await _dohodDbContext.t_dohod.AsNoTracking()
                               .Where(w => w.DateOper == dohod.DateOper
                                        && w.Guid == dohod.Guid
                                        && w.VidDoc == VidDoc.ZF
                                        && w.PlatId == dohod.PlatId
                                        && w.KbkId == dohod.KbkId
                                        && w.NumPp == dohod.NumPp
                                        && w.SumItg == dohod.SumItg)
                               .Include(x => x.Orig)
                               .ToListAsync();
             return query.FirstOrDefault();
        }

    }
}
