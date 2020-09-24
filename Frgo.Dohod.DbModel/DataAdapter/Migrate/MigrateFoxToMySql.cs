using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbfDataReader;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Data;
using Frgo.Dohod.DbModel.ModelCrm.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace Frgo.Dohod.DbModel.DataAdapter.Migrate
{
    public class MigrateFoxToMySql : IMigrateFoxToMySql
    {
        private const string OrderedTbls = "calendar,spr_okat,spr_adm,spr_lev,spr_doh,spr_dohi,spr_kceli,spr_bnk,spr_okved,spr_plat,spr_plokv,spr_sch,dohod";
 
        private DohodDbContext _dohodDbContext;
        public MigrateFoxToMySql(DohodDbContext dohodDbContext)
        {
            _dohodDbContext = dohodDbContext;
        }




        public async Task<ResultCrmDb> MigrateFromFox(string dirFoxDb, Encoding encoding)
        {
            var result = new ResultCrmDb();
            List<ExpandoObject> tbdata;
            try
            {
                foreach (var name in OrderedTbls.Split(','))
                {
                    tbdata = await ReadFoxTb(FoxFile(name), encoding);
                    switch (name)
                    {
                        case "calendar":
                            await WriteCalendar(tbdata);
                            break;
                        case "spr_okat":
                            await WriteOktmo(tbdata);
                            break;
                        case "spr_adm":
                            await WriteAdm(tbdata);
                            break;
                        case "spr_lev":
                            await WriteLev(tbdata);
                            break;
                        case "spr_doh":
                            await WriteKbk(tbdata);
                            break;
                        case "spr_dohi":
                            await WriteKbkDop(tbdata);
                            break;
                        case "spr_kceli":
                            await WriteKceli(tbdata);
                            break;
                        case "spr_bnk":
                            await WriteBank(tbdata);
                            break;
                        case "spr_okved":
                            await WriteOkved(tbdata);
                            break;
                        case "spr_plat":
                            await WriteOrg(tbdata);
                            break;
                        case "spr_plokv":
                            await WriteOrgOkved(tbdata);
                            break;
                        case "spr_sch":
                            await WriteOrgSchet(tbdata);
                            break;
                        case "dohod":
                            await WriteDohod(tbdata);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError("0", ex.Message);
            }
            return result;

            string FoxFile(string tbname) => Path.Combine(dirFoxDb, $"{tbname}.dbf");
        }



        public async Task<List<ExpandoObject>> ReadFoxTb(string file, Encoding encoding)
        {
            var tbdata = new List<ExpandoObject>();
            var skipDeleted = true;
            using (var dbfTable = new DbfTable(file, encoding))
            {
                var columns = dbfTable.Columns;
                var dbfRecord = new DbfRecord(dbfTable);

                while (dbfTable.Read(dbfRecord))
                {
                    if (skipDeleted && dbfRecord.IsDeleted)
                        continue;

                    var obj = new ExpandoObject();
                    foreach (var col in columns)
                    {
                        var val = dbfRecord.Values[col.Index].GetValue();
                        if (val == null)
                            ((IDictionary<string, object>)obj).Add(col.Name, null);
                        else if (col.ColumnType == DbfColumnType.Character)
                            ((IDictionary<string, object>)obj).Add(col.Name, (val ?? "").ToString().Trim());
                        else if (col.ColumnType == DbfColumnType.SignedLong)
                            ((IDictionary<string, object>)obj).Add(col.Name, Convert.ToInt32(val));  // (int)(long)
                        else
                            ((IDictionary<string, object>)obj).Add(col.Name, val);
                    }
                    tbdata.Add(obj);
                }
                dbfTable.Close();
            }
            return tbdata;
        }


        public async Task WriteCalendar(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(s_calendar).Name);

            foreach (dynamic obj in lst)
            {
                var calendar = new s_calendar();

                calendar.Sysid = (int)obj.SYSID;
                calendar.Date = obj.DATE;
                calendar.IsWeekend = obj.SUNDAY;
                calendar.IsNoPostDay = obj.NOPOST;
                calendar.Summa = (decimal)(obj.SUMMA ?? 0.0M);
                calendar.SummaZf = (decimal)(obj.SUM_ZF ?? 0.0M);
                calendar.SummaIp = (decimal)(obj.SUM_IP ?? 0.0M);
                calendar.SummaReestr = (decimal)(obj.SUM_REESTR ?? 0.0M);

                FillAppEditFields(obj, calendar);
                await _dohodDbContext.s_calendar.AddAsync(calendar);
            }
            await _dohodDbContext.SaveChangesAsync();
        }

        public async Task WriteOktmo(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(s_oktmo).Name);

            foreach (dynamic obj in lst)
            {
                var oktmo = new s_oktmo();

                oktmo.Sysid = (int)obj.SYSID;
                oktmo.Oktmo = obj.OKATO;
                oktmo.Name = obj.NAME_S;
                oktmo.FullName = obj.NAME;
                oktmo.CodeAte = obj.CODE_ATE;
                oktmo.CodePos = obj.CODE_POS;
                oktmo.Ls = obj.LS;
                oktmo.OkatoOld = obj.OKATOOLD;

                FillAppEditFields(obj, oktmo);
                await _dohodDbContext.s_oktmo.AddAsync(oktmo);
            }
            await _dohodDbContext.SaveChangesAsync();
        }

        public async Task WriteAdm(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(s_adm).Name);

            foreach (dynamic obj in lst)
            {
                var adm = new s_adm();

                adm.Sysid = (int)obj.SYSID;
                adm.CodeAdm = obj.KBKADM;
                adm.Inn = obj.INN;
                adm.Kpp = obj.KPP;
                adm.Name = obj.NAME1;
                adm.FullName = obj.NAME2;

                FillAppEditFields(obj, adm);
                await _dohodDbContext.s_adm.AddAsync(adm);
            }
            await _dohodDbContext.SaveChangesAsync();
        }

        public async Task WriteLev(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(s_lev).Name);

            foreach (dynamic obj in lst)
            {
                var lev = new s_lev();

                lev.Sysid = (int)obj.SYSID;
                lev.NomLevel = (int)obj.LEV_NOM;
                lev.FldSumName = obj.FLDSUMNAME;
                lev.Inn = obj.INN;
                lev.Kpp = obj.KPP;
                lev.Name = obj.NAME;
                lev.Schet = obj.SCHET;
                lev.Oktmo = obj.OKTMO;
                lev.NameBudg = obj.NAMEBUDG;

                FillAppEditFields(obj, lev);
                await _dohodDbContext.s_lev.AddAsync(lev);
            }
            await _dohodDbContext.SaveChangesAsync();
        }

        public async Task WriteKbk(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(s_kbkdop).Name);
            _dohodDbContext.TruncateDbTable(typeof(s_kbk).Name);

            foreach (dynamic obj in lst)
            {
                var kbk = new s_kbk();

                kbk.Sysid = (int)obj.SYSID;
                kbk.Kbk = obj.KBK;
                kbk.OktmoId = (obj.ID_OKATO ?? 0) > 0 ? obj.ID_OKATO : null;
                kbk.DateBeg = obj.DATE1;
                kbk.DateEnd = obj.DATE2;
                kbk.ProcFed = (double)(obj.PR_FED ?? 0.0);
                kbk.ProcObl = (double)(obj.PR_OBL ?? 0.0);
                kbk.ProcMest = (double)(obj.PR_MESTN ?? 0.0);
                kbk.ProcPos1 = (double)(obj.PR_DOP1 ?? 0.0);
                kbk.ProcPos2 = (double)(obj.PR_DOP2 ?? 0.0);
                kbk.ProcFond = (double)(obj.PR_FOND ?? 0.0);
                kbk.ProcSmol = (double)(obj.PR_SMLN ?? 0.0);
                kbk.NumBudgKop = (int)(obj.NUMBUDGKOP ?? 0);
                kbk.IsPriznak = !string.IsNullOrWhiteSpace(obj.PRIZNAK);
                kbk.IsItog = !string.IsNullOrWhiteSpace(obj.ITOG);
                kbk.IsGr = obj.GR ?? false;
                kbk.DateRad = obj.DATE_RA;
                kbk.Name = obj.NAMESMALL;
                kbk.FullName = obj.NAME;

                FillAppEditFields(obj, kbk);
                await _dohodDbContext.s_kbk.AddAsync(kbk);
            }
            await _dohodDbContext.SaveChangesAsync();
        }

        public async Task WriteKbkDop(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(s_kbkdop).Name);

            foreach (dynamic obj in lst)
            {
                var codekbk = (string)obj.KBK;
                var kbkdop = new s_kbkdop();

                kbkdop.Sysid = (int)obj.SYSID;
                kbkdop.OktmoId = (obj.OKATOID ?? 0) > 0 ? obj.OKATOID : null;
                kbkdop.DateBeg = obj.DATE1;
                kbkdop.DateEnd = obj.DATE2;
                kbkdop.ProcFed = (double)(obj.PR_FED ?? 0.0);
                kbkdop.ProcObl = (double)(obj.PR_OBL ?? 0.0);
                kbkdop.ProcMest = (double)(obj.PR_MESTN ?? 0.0);
                kbkdop.ProcPos1 = (double)(obj.PR_DOP1 ?? 0.0);
                kbkdop.ProcPos2 = (double)(obj.PR_DOP2 ?? 0.0);
                kbkdop.ProcFond = (double)(obj.PR_FOND ?? 0.0);
                kbkdop.ProcSmol = (double)(obj.PR_SMLN ?? 0.0);
                kbkdop.NumBudgKop = (int)(obj.NUMBUDGKOP ?? 0);
                kbkdop.IsPriznak = !string.IsNullOrWhiteSpace(obj.PRIZNAK);
                kbkdop.IsItog = !string.IsNullOrWhiteSpace(obj.ITOG);
                kbkdop.Name = obj.NAMESMALL;
                kbkdop.FullName = obj.NAME;
                var parent = _dohodDbContext.s_kbk.Where(w => w.Kbk == codekbk).FirstOrDefault();
                if (parent == null)
                    continue;
                kbkdop.IsGr = parent.IsGr; ;
                kbkdop.DateRad = parent.DateRad;
                kbkdop.Kbk = parent;

                FillAppEditFields(obj, kbkdop);
                await _dohodDbContext.s_kbkdop.AddAsync(kbkdop);
            }
            await _dohodDbContext.SaveChangesAsync();
        }

        public async Task WriteKceli(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(s_kceli).Name);

            foreach (dynamic obj in lst)
            {
                var kceli = new s_kceli();

                kceli.Sysid = (int)obj.SYSID;
                kceli.CodeCeli = obj.KODCELI;
                kceli.Name = obj.NAMECELI;
                kceli.FullName = obj.NAMECELI;
                kceli.DateBeg = obj.DATE_BEG;

                FillAppEditFields(obj, kceli);
                await _dohodDbContext.s_kceli.AddAsync(kceli);
            }
            await _dohodDbContext.SaveChangesAsync();
        }

        public async Task WriteBank(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(s_bank).Name);

            foreach (dynamic obj in lst)
            {
                var bank = new s_bank();

                bank.Sysid = (int)obj.SYSID;
                bank.Name = obj.NAME;
                bank.FullName = obj.NAME;
                bank.Bic = obj.BIC;
                bank.Town = obj.GOROD;
                bank.IsDepart = false;
                bank.DateClosed = null;

                FillAppEditFields(obj, bank);
                await _dohodDbContext.s_bank.AddAsync(bank);
            }
            await _dohodDbContext.SaveChangesAsync();
        }

        public async Task WriteOkved(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(s_okved).Name);
            _dohodDbContext.TruncateDbTable(typeof(s_orgokved).Name);

            foreach (dynamic obj in lst)
            {
                var okved = new s_okved();

                okved.Sysid = (int)obj.SYSID;
                okved.Code = obj.CODE;
                okved.CodeOkved = obj.KOD;
                okved.Name = obj.NAMESHORT;
                okved.FullName = obj.NAME;
                okved.InfoText = obj.INFO;
                okved.Primechanie = obj.PRIM;

                FillAppEditFields(obj, okved);
                await _dohodDbContext.s_okved.AddAsync(okved);
            }
            await _dohodDbContext.SaveChangesAsync();
        }

        public async Task WriteOrg(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(s_org).Name);
            _dohodDbContext.TruncateDbTable(typeof(s_orgokved).Name);

            foreach (dynamic obj in lst)
            {
                var org = new s_org();

                org.Sysid = (int)obj.SYSID;
                org.Inn = obj.INN;
                org.Kpp = obj.KPP;
                org.Oktmo = "";
                org.Name = obj.NAME_S;
                org.FullName = obj.NAME;
                org.TypeOrg = (int)(obj.TYPEPLAT ?? 0);
                org.Dop1Id = (obj.ID_DOP1 ?? 0) > 0 ? obj.ID_DOP1 : null;
                org.Dop2Id = (obj.ID_DOP2 ?? 0) > 0 ? obj.ID_DOP2 : null;
                org.Dop3Id = (obj.ID_DOP3 ?? 0) > 0 ? obj.ID_DOP3 : null;
                org.RukDolzn = obj.RUKDOLZN;
                org.Rukovod = obj.RUKOVOD;
                org.Glavbuh = obj.GLAVBUH;
                org.Phone = obj.PHONE;
                org.Fax = obj.FAX;
                org.AdresJure = obj.ADDR_JURE;
                org.AdresFakt = obj.ADDR_FACT;

                FillAppEditFields(obj, org);
                await _dohodDbContext.s_org.AddAsync(org);
            }
            await _dohodDbContext.SaveChangesAsync();
        }

        public async Task WriteOrgOkved(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(s_orgokved).Name);

            foreach (dynamic obj in lst)
            {
                if (obj.ID_PLAT == null)
                    continue;
                var idorg = (int)obj.ID_PLAT;
                var org = _dohodDbContext.s_org.Where(w => w.Sysid == idorg).FirstOrDefault();
                if (org == null)
                    continue;

                var orgokved = new s_orgokved();
                orgokved.Sysid = (int)obj.SYSID;
                orgokved.OrgId = idorg;
                orgokved.Okved = obj.OKVED;
                orgokved.IsOsn = (bool)(obj.F_OSN ?? false);

                FillAppEditFields(obj, orgokved);
                await _dohodDbContext.s_orgokved.AddAsync(orgokved);
            }
            await _dohodDbContext.SaveChangesAsync();
        }

        public async Task WriteOrgSchet(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(s_schet).Name);

            foreach (dynamic obj in lst)
            {
                if (obj.ID_PLAT == null)
                    continue;
                var idorg = (int)obj.ID_PLAT;
                var org = _dohodDbContext.s_org.AsNoTracking().Where(w => w.Sysid == idorg).FirstOrDefault();
                if (org == null)
                    continue;

                int? idbank = null;
                if (obj.ID_BNK != null)
                {
                    idbank = (int)obj.ID_BNK;
                    var bank = _dohodDbContext.s_bank.AsNoTracking().Where(w => w.Sysid == idbank).FirstOrDefault();
                    idbank = bank?.Sysid;
                }

                var schet = new s_schet();
                schet.Sysid = (int)obj.SYSID;
                schet.OrgId = idorg;
                schet.BankId = idbank;
                schet.Rs = obj.RSCHET;

                FillAppEditFields(obj, schet);
                await _dohodDbContext.s_schet.AddAsync(schet);
            }
            await _dohodDbContext.SaveChangesAsync();
        }



        private async Task WriteDohod(List<ExpandoObject> lst)
        {
            _dohodDbContext.TruncateDbTable(typeof(t_dohorig).Name);
            _dohodDbContext.TruncateDbTable(typeof(t_dohod).Name);

            foreach (dynamic obj in lst)
            {
                t_dohod dohod = new t_dohod() { LinkVoz = null, LinkNp = null, LinkItg = null };
                t_dohorig dohorig = new t_dohorig();

                var codeDoc = (int)(obj.CODE_DOC ?? 0);
                var sysid = (int)obj.SYSID;
                var linkNp = (int?)obj.LINK_NP ?? 0;
                var linkVoz = (int?)obj.LINK_VOZ ?? 0;
                var linkItg = (int?)obj.LINK_ITG ?? 0;
                var isNp = linkNp != 0;
                var isVoz = linkVoz != 0;
                var isItg = linkItg == -1;


                // dohorig =
                dohorig.Oktmo = obj.OKATOFL;
                dohorig.OktmoPp = obj.OKATO; // (поле 105)*
                dohorig.Kbk = obj.KBK;
                dohorig.KbkPp = obj.KBKPP;   // (поле 104)*
                dohorig.CodeCeli = obj.KCELI;
                dohorig.Inn = obj.INN;
                dohorig.Kpp = obj.KPP;
                dohorig.PlatName = obj.PLAT_NAME;
                dohorig.Rs = obj.RSCHET;
                dohorig.Bic = obj.BIC;
                dohorig.KorSchet = obj.KOR_SCHET;
                dohorig.BankName = obj.BANK_NAME;

                var xx = (string)(obj.INN);

                // dohod
                dohod.Date = obj.DATE;
                dohod.DateOper = obj.DATE_OPER;
                dohod.NumPp = obj.NUMPP;
                dohod.DatePp = obj.DATEPP;
                dohod.NumZf = obj.NOM_ZF;
                dohod.DateZf = obj.DATE_ZF;
                dohod.OktmoId = (obj.ID_OKATO ?? 0) > 0 ? obj.ID_OKATO : null; // (await _dohodDbContext.GetOkttoOnCode(dohorig.Oktmo))?.Sysid;
                dohod.KbkId = (obj.ID_KBK ?? 0) > 0 ? obj.ID_KBK : null; // (await _dohodDbContext.IdentKbk(dohorig.Kbk))?.Sysid;
                dohod.PlatId = (obj.ID_PLAT ?? 0) > 0 ? obj.ID_PLAT : null; // (await _dohodDbContext.IdentPlat(dohorig))?.Sysid; ;
                dohod.CodeCeliId = (await _dohodDbContext.IdentKceli(dohorig.CodeCeli))?.Sysid;
                dohod.IsRaschet = obj.RASCH ?? false;
                dohod.IsSumRazb = obj.SUMRAZBN ?? false;
                dohod.SumItg = (decimal)(obj.SUM_ITOG ?? 0.0M);
                dohod.SumFed = (decimal)(obj.SUM_FED ?? 0.0M);
                dohod.SumObl = (decimal)(obj.SUM_OBL ?? 0.0M);
                dohod.SumMest = (decimal)(obj.SUM_MEST ?? 0.0M);
                dohod.SumPgt = (decimal)(obj.SUM_DOP1 ?? 0.0M);
                dohod.SumPst = (decimal)(obj.SUM_DOP2 ?? 0.0M);
                dohod.SumFond = (decimal)(obj.SUM_FOND ?? 0.0M);
                dohod.SumSmol = (decimal)(obj.SUM_SMLN ?? 0.0M);
                dohod.ImnsInn = obj.IMNS_INN;
                dohod.ImnsKpp = obj.IMNS_KPP;
                dohod.LsAdb = obj.LS_ADB;
                dohod.OsnPlat = obj.OSN_PLAT;     // (поле 106)*
                dohod.CodeVidOper = codeDoc == 0 ? "" : codeDoc.ToString().PadLeft(2, '0');
                dohod.VidOper = obj.VIDOPER;
                dohod.NaznPlat = obj.NAZNPLAT;   // Назначение платежа
                dohod.ImportFile = obj.IMPFILE;

                dohod.IsVoz = isVoz;
                if (linkVoz > 0)
                    dohod.LinkVoz = linkVoz;
                dohod.IsNp = isNp;
                if (linkNp > 0)
                    dohod.LinkNp = linkNp;
                dohod.IsItg = isItg;
                if (linkItg > 0)
                    dohod.LinkItg = linkItg;

                dohod.Guid = obj.GUID;
                dohod.VidDoc = (VidDoc)(obj.VIDDOC ?? 0);
                if (dohod.VidDoc == VidDoc.CE && Path.GetExtension(dohod.ImportFile).ToUpper().StartsWith(".SE"))
                    dohod.VidDoc = VidDoc.SE;
                dohod.Sysid = sysid;
                dohorig.Sysid = sysid;
                dohorig.DohodId = sysid;

                FillAppEditFields(obj, dohod);
                FillAppEditFields(obj, dohorig);
                await _dohodDbContext.t_dohod.AddAsync(dohod);
                await _dohodDbContext.t_dohorig.AddAsync(dohorig);
            }
            await _dohodDbContext.SaveChangesAsync();

            return;
        }


        private void FillAppEditFields(dynamic obj, BaseModel basemodel)
        {
            if (((IDictionary<string, object>)obj).ContainsKey("CRTDATE"))
                basemodel.AppDate = obj.CRTDATE;
            if (((IDictionary<string, object>)obj).ContainsKey("CRTUID"))
                basemodel.AppUser = (obj.CRTUID ?? 0) > 0 ? obj.CRTUID : null;

            if (((IDictionary<string, object>)obj).ContainsKey("EDITDATE"))
                basemodel.EditDate = obj.EDITDATE;
            if (((IDictionary<string, object>)obj).ContainsKey("EDITUID"))
                basemodel.EditUser = (obj.EDITUID ?? 0) > 0 ? obj.EDITUID : null;
        }
    }
}
