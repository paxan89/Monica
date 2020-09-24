using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Fk;
using System;
using System.Linq;
using System.Reflection;
using Monica.Core.DataBaseUtils;
using Frgo.Dohod.DbModel.ModelCrm;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.ModelCrm.Dictionary;
using System.Runtime.InteropServices;
using Frgo.Dohod.DbModel.ModelCrm.Data;
using System.Collections.Generic;
using Frgo.Dohod.DbModel.Extension.Txt;
using System.Globalization;

namespace Frgo.Dohod.DbModel.Extension
{
    public static class Fn
    {
        public static (DateTime, DateTime) GetMinMax(this DateTime? date, DateTime datemin, DateTime datemax)
        {
            // если null остается прежние значения
            if (date != null)
            {
                if (datemin == default(DateTime) || date < datemin)
                    datemin = date ?? datemin;
                if (datemax == default(DateTime) || date > datemax)
                    datemax = date ?? datemax;
            }
            return (datemin, datemax);
        }


        public static void SetTheOktmo(this DohodDbContext dbContext, ImportParam importParam, ResultCrmDb result)
        {
            //dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            importParam.TheOktmo = dbContext.s_oktmo.AsNoTracking().Where(x => x.CodeAte == importParam.CodeAte && x.CodePos == importParam.CodePos).FirstOrDefault();
            if(importParam.TheOktmo == null)
               result.AddError(Constant.ErrCodeSetting, "Не найден ОКТМО. Возможно, не правильные параметры настройки");
        }


        public static bool IsNpCode(this string kbk)
        {
            // так быстрее, чем исползовать Regex и NpCodeMask
            return kbk.Length == 20 && kbk.Substring(3, 5) == "11701"; 
        }


        public static string GetVidOperName(this string codeVidOper)
        {
            return codeVidOper == "01" ? "Платежное поручение" :
                   codeVidOper == "02" ? "Платежное требование" :
                   codeVidOper == "06" ? "Инкассовое поручение" :
                   codeVidOper == "16" ? "Платежный ордер" :    codeVidOper;    
        }


        public static string  ImportFileMask(this ImportParam importParam)
        {
            return importParam.FormatGroup.EndsWith("XL") ? "*.xlsx" : $"*.{importParam.Format}?";
        }



        public static dynamic ToFkTypeVal(this string strval, fk_code fkcode)
        {
            dynamic val = null;
            switch (fkcode.FldType)
            {
                case "STRING":
                case "GUID":
                    val = strval.Trim();
                    break;

                case "DATE":
                    if (!string.IsNullOrWhiteSpace(strval))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(strval, out dt))
                            val = dt;
                    }
                    break;

                case "OADATE": //  (OLE Automation Date) 
                    if (!string.IsNullOrWhiteSpace(strval))
                    {
                        double oaDate;
                        if (double.TryParse(strval, out oaDate))
                            val = DateTime.FromOADate(oaDate);
                    }
                    break;

                case "NUMBER":
                    int valint;
                    if (int.TryParse(strval, out valint))
                        val = (int)valint;
                    break;

               case "NUMBER2":
                    decimal valdec;
                    if (decimal.TryParse(strval.Trim(), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, Constant.CU, out valdec))
                        val = valdec;
                    else if (decimal.TryParse(strval.Trim().Replace(',', '.'), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, Constant.CU, out valdec))
                        val = valdec;
                    //else   - пусть лучше Null - по ошибке можнл будет отловить
                    //    val = 0.0M;
                    break;

                default:
                    val = strval;
                    break;
            }
            return val;
        }


        public static void TruncateDbTable(this DohodDbContext dbContext, string table)
        {
            dbContext.Database.GetDbConnection()
                     .ExecuteScalar($"SET FOREIGN_KEY_CHECKS = 0; TRUNCATE TABLE {table}; ALTER TABLE {table} AUTO_INCREMENT=1; SET FOREIGN_KEY_CHECKS = 1");
        }
     

        public static DateTime GetSkipedWorkDay(this DohodDbContext dbContext, DateTime date, int daysToSkip)
        {
            DateTime? dateSkiped = null;
            if(daysToSkip>=0)
                dateSkiped = dbContext.s_calendar.AsNoTracking().Where(x => x.Date >= date && !(x.IsWeekend ?? false) && !(x.IsNoPostDay ?? false))
                                      .OrderBy(x => x.Date).Skip(daysToSkip).Take(1).FirstOrDefault().Date ;
            else
                dateSkiped = dbContext.s_calendar.AsNoTracking().Where(x => x.Date <= date && !(x.IsWeekend ?? false) && !(x.IsNoPostDay ?? false))
                                      .OrderByDescending(x => x.Date).Skip(Math.Abs(daysToSkip)).Take(1).FirstOrDefault().Date ;
             return dateSkiped ?? date;
        }


        public static async Task<s_org> IdentPlat(this DohodDbContext dbContext, t_dohorig dohorig)
        {
            return await IdentPlat(dbContext, dohorig.Inn, dohorig.Kpp, dohorig.PlatName, dohorig.Rs);
        }
        public static async Task<s_org> IdentPlat(this DohodDbContext dbContext, string inn, string kpp, string name, string rs, bool isToAdd = true)
        {
             //if(dbContext.s_org.Local.Count==0)
            //   await  dbContext.s_org.LoadAsync();
            s_org org = null;
            if (!string.IsNullOrWhiteSpace(inn) && inn != "0" &&
                !string.IsNullOrWhiteSpace(kpp) && kpp != "0")
                org = dbContext.s_org.AsNoTracking().Where(x => x.Inn == inn && x.Kpp == kpp).FirstOrDefault();

            else if (!string.IsNullOrWhiteSpace(inn) && inn != "0" &&
                     (string.IsNullOrWhiteSpace(kpp) || kpp == "0"))
            {
                // Сначала поиск только среди с пустым kpp
                org = dbContext.s_org.AsNoTracking().Where(x => x.Inn == inn && (x.Kpp == "" || x.Kpp == "0")).OrderBy(x => x.Sysid).FirstOrDefault();
                if (org == null)
                    org = dbContext.s_org.AsNoTracking().Where(x => x.Inn == inn).OrderBy(x => x.Sysid).FirstOrDefault();
            }
            else if (string.IsNullOrWhiteSpace(inn) && string.IsNullOrWhiteSpace(name))
                // Для пустых ИНН  и Name == ЭТО == "Неизвестный плательщик" тип ему:2 - частное лицо
                // КПП -не учитываем, считаем пустым
                org = dbContext.s_org.AsNoTracking().Where(x => x.Inn == "" && x.Kpp == "" && x.Name == Constant.UnknownOrg).FirstOrDefault();

            else if (string.IsNullOrWhiteSpace(inn) && name == Constant.VidDocKorOrg)
                // Для пустых ИНН  и Name = VidDocKorOrg == ЭТО == "Корректирующий платеж" тип ему:2 - частное лицо
                // КПП -не учитываем, считаем пустым
                org = dbContext.s_org.AsNoTracking().Where(x => x.Inn == "" && x.Kpp == "" && x.Name == Constant.VidDocKorOrg).FirstOrDefault();

            else if ((string.IsNullOrWhiteSpace(inn) || inn == "0") && !string.IsNullOrWhiteSpace(name))
            {
                // Для пустых ИНН по наименованию(с учетом КПП если есть)
                if (string.IsNullOrWhiteSpace(kpp) || kpp == "0")
                    org = dbContext.s_org.AsNoTracking().Where(x => (x.Inn == "" || x.Inn == "0") &&
                                                                    (x.Kpp == "" || x.Kpp == "0") &&
                                                                    (x.Name == name || x.FullName == name)).FirstOrDefault();
                else
                    org = dbContext.s_org.AsNoTracking().Where(x => (x.Inn == "" || x.Inn == "0") &&
                                                                    (x.Kpp == kpp) &&
                                                                    (x.Name == name || x.FullName == name)).FirstOrDefault();
            }
            var lll = name.Length;
            if (org == null && isToAdd)
                org = await dbContext.AddOrg(inn, kpp, name, rs);

            return org;
        }

        public static async Task<s_lev> IdentLevelOnSchet(this DohodDbContext dbContext, string rs)
        {
            if (string.IsNullOrWhiteSpace(rs))
                return null;
            var levs = await dbContext.s_lev.AsNoTracking().Where(x => x.Schet == rs).ToListAsync();
            return levs.Count==1 ? levs[0] : null;
        }

        public static async Task<s_lev> IdentLevelOnNameBudget(this DohodDbContext dbContext, string namebudget)
        {
            if (string.IsNullOrWhiteSpace(namebudget))
                return null;
            var name = namebudget.ToCompareString();
            var levs = await dbContext.s_lev.AsNoTracking().Where(x => x.NameBudg.ToCompareString() == name).ToListAsync();
            return levs.Count==1 ? levs[0] : null;
        }

        public static async Task<t_uvdet> IdentUved(this DohodDbContext dbContext, t_dohod dohod, ImportParam importParam, bool isUvDocMarking = true)
        {
            // пытаемся определить плательщика, через уведомление
            var uvdets = await dbContext.t_uvdet.AsNoTracking()
                           .Where(w => w.Uv.DateUv == dohod.DatePp
                                    && w.Uv.NomUv == dohod.NumPp
                                    && w.Kbk == dohod.Orig.Kbk
                                    && Math.Abs(w.Summa) == Math.Abs(dohod.SumItg))
                           .ToListAsync();
            // Как быстрее ??
            //var query = await _dohodDbContext.t_uvdet.Where(w=>w.Kbk == dohod.Orig.Kbk && w.Summa == dohod.SumItg)
            //            .Join(_dohodDbContext.t_uv.Where(w=>w.DateUv == dohod.DatePp && w.NomUv == dohod.NumPp), x=>x.UvId, y=>y.Sysid, (x,y)=>x.UvId)
            //            .ToListAsync();

            if (uvdets.Count == 1)
            {
                var uvdet0 = uvdets[0]; // Это может быть как NevPost так и !NevPost
                                        // плательщика берем из уведомления и для VT и для SF, но там его id пока нет
                var uvdetnp = await dbContext.t_uvdet.AsNoTracking()
                                   .Where(w => w.Uv.Sysid == uvdet0.UvId && (w.IsNp ?? false))
                                   .FirstOrDefaultAsync();
                if (uvdetnp != null && dohod.Sysid != 0)
                {
                    var org = await dbContext.IdentPlat(uvdetnp.PlatInn, uvdetnp.PlatKpp, uvdetnp.PlatName, "");
                    if (org != null)
                    {
                        // ---Запись в t_dohod -----
                        dohod.PlatId = org.Sysid;
                        dohod.Orig.Inn = org.Inn;
                        dohod.Orig.Kpp = org.Kpp;
                        dohod.Orig.PlatName = org.Name;
                        dohod.LinkNp = uvdetnp.UvId;
                    }
                }
                if (isUvDocMarking)
                {
                    // ---Запись в t_uvdet -----
                    uvdet0.DateVyp = dohod.Date;
                    dbContext.t_uvdet.Update(uvdet0);
                }
                await dbContext.SaveChangesAsync();
                return uvdet0;
            }
            return null;
        }



        public static async Task<s_org> AddOrg(this DohodDbContext dbContext, string inn, string kpp, string name, string rs)
        {
             // Внести изменение в пополнение справочника Плательщики. 20150406
            // Если плательщиком является организация(банк) с реквизитами ИНН и КПП юр.лица, 
		    // а в наименовании плательщика дополнительно(//.....) указывается ФИО физ.лица,
            // то в справочник Плательщики в наименовании записывать имя без дополнительных реквизитов
            var nameForDictionary = name;
            if (inn.Length == 10 && kpp.Length == 9)
                nameForDictionary = name.Split("//")[0];
            var typePlat = inn.Length == 10 ? 1 :2;
            var org = new s_org()
            {
                Inn = inn,
                Kpp = kpp,
                Name = nameForDictionary,
                FullName = nameForDictionary,
                TypeOrg = 0,
                AdresJure = "", // в фоксе при импорте не записывались
                AdresFakt = ""  // в фоксе при импорте не записывались
            };
            var res = await dbContext.s_org.AddAsync(org);
            await dbContext.SaveChangesAsync(); // тест
            org.Sysid = res.Entity.Sysid;

            if (!string.IsNullOrWhiteSpace(rs))
            {
                var schet = new s_schet()
                {
                    OrgId = org.Sysid,
                    BankId = null, // в фоксе банки не записывались
                    Rs = rs
                };
                await dbContext.s_schet.AddAsync(schet);
                await dbContext.SaveChangesAsync();
           }
            return org;
        }


        public static async Task<s_kbk> IdentKbk(this DohodDbContext dbContext, string kbkcode)
        {
            var kbk = dbContext.s_kbk.AsNoTracking().Where(x => x.Kbk == kbkcode).FirstOrDefault();

            if (kbk == null)          
                kbk = await dbContext.AddKbk(kbkcode);

            return kbk;
        }


        public static async Task<s_kbk> AddKbk(this DohodDbContext dbContext, string kbkcode)
        {
            var kbk = new s_kbk()
            {
                Kbk = kbkcode,
                OktmoId = null,
                DateBeg = null,
                DateEnd = null,
                ProcFed = 100.0,
                Name = $"Добавлено при импорте {DateTime.Now:dd.MM.yyyy}",
                FullName = $"Добавлено при импорте {DateTime.Now}",
                IsPriznak = false
            };
            var res = await dbContext.s_kbk.AddAsync(kbk);
            await dbContext.SaveChangesAsync();
            kbk.Sysid = res.Entity.Sysid;

            return kbk;
        }


        public static async Task<s_kceli> IdentKceli(this DohodDbContext dbContext, string codeceli)
        {
            if (string.IsNullOrWhiteSpace(codeceli))
                return null;

            var kceli = dbContext.s_kceli.AsNoTracking().Where(x => x.CodeCeli == codeceli).FirstOrDefault();
            if (kceli == null)
                kceli = await dbContext.AddKceli(codeceli);

            return kceli;
        }

        public static async Task<s_kceli> AddKceli(this DohodDbContext dbContext, string codeceli)
        {
            var kceli = new s_kceli()
            {
                CodeCeli = codeceli,
                DateBeg = null,
                Name = $"Добавлено при импорте {DateTime.Now:dd.MM.yyyy}",
                FullName = $"Добавлено при импорте {DateTime.Now}",
            };

            var res = await dbContext.s_kceli.AddAsync(kceli);
            await dbContext.SaveChangesAsync();
            kceli.Sysid = res.Entity.Sysid;
            return kceli;
        }
        public static async Task<s_oktmo> GetOkttoOnCode(this DohodDbContext dbContext, string codeoktmo)
        {
            if (string.IsNullOrWhiteSpace(codeoktmo))
                return null;

            var oktmo = dbContext.s_oktmo.AsNoTracking().Where(x => x.Oktmo == codeoktmo).FirstOrDefault();
 
            return oktmo;
        }
        public static async Task<bool> RazbivkaItogSum(this DohodDbContext dbContext, t_dohod dohod)
        {
            if (dohod == null)
                return false;

            if(dohod.IsSumRazb ?? false)
            {
                // НЕ стандартная разбивка - просто зануляется суммы бюджетов
                dohod.SetEmptyBudgetSums();
                return true;
            }

            if ((dohod.KbkId ?? 0) == 0)
                return false;

            var sumitg = dohod.SumItg;
            var dic = new Dictionary<string, decimal>();
            // Ищем сочетание КБК + ОКАТО + ПЕРИОД (в s_dohdop)
            var proc = await dbContext.s_kbkdop.Where(x
                        => x.KbkId == dohod.KbkId
                        && (x.OktmoId == null || x.OktmoId == dohod.OktmoId)
                        && (x.DateBeg == null || x.DateBeg <= dohod.DateOper)
                        && (x.DateEnd == null || x.DateEnd >= dohod.DateOper))
                .Select(x => new
                {
                    x.ProcFed,
                    x.ProcObl,
                    x.ProcMest,
                    x.ProcPos1,
                    x.ProcPos2,
                    x.ProcFond,
                    x.ProcSmol,
                    x.NumBudgKop
                }).FirstOrDefaultAsync(); // .ToListAsync();

            if(proc==null)
                proc = await dbContext.s_kbk.Where(x=> x.Sysid == dohod.KbkId)                  
                .Select(x => new
                {
                    x.ProcFed,
                    x.ProcObl,
                    x.ProcMest,
                    x.ProcPos1,
                    x.ProcPos2,
                    x.ProcFond,
                    x.ProcSmol,
                    x.NumBudgKop
                }).FirstOrDefaultAsync();

            if (proc!=null)
            {
                // Расчет процентов
                dic.Add("SumFed", Math.Round(sumitg * (decimal)proc.ProcFed/100, 2));
                dic.Add("SumObl", Math.Round(sumitg * (decimal)proc.ProcObl/100, 2));
                dic.Add("SumMest", Math.Round(sumitg * (decimal)proc.ProcMest/100, 2));
                dic.Add("SumPgt", Math.Round(sumitg * (decimal)proc.ProcPos1/100, 2));
                dic.Add("SumPst", Math.Round(sumitg * (decimal)proc.ProcPos2/100, 2));
                dic.Add("SumFond", Math.Round(sumitg * (decimal)proc.ProcFond/100, 2));
                dic.Add("SumSmol", Math.Round(sumitg * (decimal)proc.ProcSmol/100, 2));
                //
                // Коррекция по плавающей копейке
                var nelem = Math.Min( Math.Max(proc.NumBudgKop - 1 ?? 0, 0), dic.Count-1);
                var key = dic.ElementAt(nelem).Key;
                if (dic[key] == 0.0M) // не будем вешать на нулевую сумму
                    key = dic.FirstOrDefault(x => x.Value != 0.0M).Key;
                if (key != null)
                {
                    var sum = dic.Aggregate(0.0M, (a, b) => a + (b.Key == key ? 0.0M : b.Value));
                    dic[key] = sumitg - sum;
                }
                // Запись сумм
                dohod.SumFed = dic["SumFed"];
                dohod.SumObl = dic["SumObl"];
                dohod.SumMest = dic["SumMest"];
                dohod.SumPgt = dic["SumPgt"];
                dohod.SumPst = dic["SumPst"];
                dohod.SumFond = dic["SumFond"];
                dohod.SumSmol = dic["SumSmol"];
            }
            else
            {
                // нет КБК
                dohod.IsSumRazb = true; // Установи прзнак нестандартной
                dohod.SetEmptyBudgetSums();
                return false;
            }
            
            return true;
        }

        private static void SetEmptyBudgetSums(this t_dohod dohod)
        {
            dohod.SumFed = 0.0M;
            dohod.SumObl = 0.0M;
            dohod.SumMest = 0.0M;
            dohod.SumPgt = 0.0M;
            dohod.SumPst = 0.0M;
            dohod.SumFond = 0.0M;
            dohod.SumSmol = 0.0M;
        }
    }
}
