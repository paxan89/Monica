﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.Extension.Txt;
using Frgo.Dohod.DbModel.Manager.Parse;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Data;
using Frgo.Dohod.DbModel.ModelCrm.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.Format
{
    public class ParserFkTxCe : BaseParserFormat, IParserFormat
    {
        // private DohodDbContext _dohodDbContext;
        public ParserFkTxCe(DohodDbContext dohodDbContext) : base(dohodDbContext)
        {
        }
        public async Task<object> ParserFormat(object doc, ImportParam importParam)
        {
            await DeleteTheFileData(importParam);

            var dohods = new List<t_dohod>();
            dynamic objCE = null;
            s_lev lev = null;
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                switch (obj.Marker)
                {
                    case "CE":
                        objCE = obj;
                        lev = await _dohodDbContext.IdentLevelOnSchet((string)objCE.BS);
                        break;

                    case "CED":
                        if (objCE == null)
                            continue;
                        if (!OkSchet())
                            continue;

                        var dohod = await DohRecord(lev, objCE, obj, importParam);
                        dohod = await ChechRecord(dohod, importParam);
                        if (dohod == null)
                            continue;
                        dohods.Add(dohod);
                        break;
                }
            }
            return dohods;

            bool OkSchet() => lev != null && !string.IsNullOrWhiteSpace(lev.Inn) && !string.IsNullOrWhiteSpace(lev.Kpp);
        }


        private async Task<t_dohod> DohRecord(s_lev lev, dynamic objCE, dynamic obj, ImportParam importParam)
        {
            t_dohod dohod = new t_dohod();
            t_dohorig dohorig = new t_dohorig();
            if (obj.TYPE_KBK != "20")
                return null;
            if (lev == null)
                return null;

            // вспомогательные
            var oktmopp = lev?.Oktmo ?? "";
            //  Для IP пока не фильтруем по m.o.idOkato = 0
            var oktmo = GetOktmo(oktmopp.Substr(0, 8), importParam);

            // dohorig 
            dohorig.Oktmo = oktmo?.Oktmo;
            dohorig.OktmoPp = oktmopp; // (поле 105)*
            dohorig.Kbk = obj.KBK;
            dohorig.KbkPp = obj.KBK;   // (поле 104)*
            dohorig.CodeCeli = obj.ADD_KLASS;
            dohorig.Inn = "";
            dohorig.Kpp = "";
            dohorig.PlatName = "";
            dohorig.Rs = lev?.Schet ?? "";
            dohorig.Bic = "";
            dohorig.BankName = "";

            // dohod
            dohod.DateOper = objCE.DATE_OTCH;
            dohod.Date = dohod.DateOper; // _dohodDbContext.GetSkipedWorkDay(dohod.DateOper ?? default(DateTime), importParam.DaysToSkip);
            dohod.NumPp = "";
            dohod.DatePp = dohod.DateOper;
            dohod.NumZf = null;
            dohod.DateZf = null;
            dohod.OktmoId = oktmo?.Sysid;
            dohod.KbkId = (await _dohodDbContext.IdentKbk(dohorig.Kbk)).Sysid;
            dohod.CodeCeliId = (await _dohodDbContext.IdentKceli(dohorig.CodeCeli))?.Sysid;
            dohod.PlatId = null;
            dohod.IsRaschet = false; // ДЛЯ IP
            dohod.IsSumRazb = true;  // true - Нестандартная разбивка по бюджетам
            var sumPost = (decimal)obj.SUM_POST;
            var sumVib = (decimal)obj.SUM_VIP;
            dohod.SumItg = sumPost - sumVib;
            dohod.ImnsInn = lev == null ? "" : lev.Inn;
            dohod.ImnsKpp = lev == null ? "" : lev.Kpp;
            dohod.LsAdb = "";
            dohod.OsnPlat = "";     // (поле 106)*
            dohod.CodeVidOper = "";
            dohod.VidOper = obj.NAME_DPPO;
            dohod.NaznPlat = "";   // Назначение платежа
            dohod.ImportFile = importParam.FileName;
            dohod.IsNp = dohorig.Kbk.IsNpCode();
            dohod.IsVoz = false;
            dohod.IsItg = false;
            dohod.LinkNp = null;
            dohod.LinkVoz = null;
            dohod.LinkItg = null;
            dohod.Guid = "";
            dohod.VidDoc = VidDoc.CE;
            //
            dohod.Orig = dohorig;
            await _dohodDbContext.RazbivkaItogSum(dohod);

            return dohod;
        }



        protected override async Task<t_dohod> GetExistingRecord(t_dohod dohod)
        {
            //  ЗАПИСЬ: НЕ ПРОВЕРЯЕТСЯ НАЛИЧИЕ ДРУГИХ ТИПОВ              
            return null;
        }
    }
}