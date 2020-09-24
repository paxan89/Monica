using Monica.Core.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Dictionary;
using Frgo.Dohod.DbModel.ModelCrm.Data;
using Frgo.Dohod.DbModel.ModelCrm.Fk;
using Microsoft.EntityFrameworkCore;
using Monica.Core.DataBaseUtils;
using Frgo.Dohod.DbModel.ModelCrm.Profile;
using Monica.Core.DbModel.ModelCrm.EngineReport;

namespace Frgo.Dohod.DbModel.ModelCrm
{
    public class DohodDbContext : BaseDbContext
    {
        /// <summary>
        /// календарь
        /// </summary>
        public DbSet<s_calendar> s_calendar { get; set; }

        /// <summary>
        /// Справочник ОКТМО
        /// </summary>
        public DbSet<s_oktmo> s_oktmo { get; set; }

        /// <summary>
        /// Справочник кодов глав
        /// </summary>
        public DbSet<s_adm> s_adm { get; set; }

        /// <summary>
        /// Справочник уровней бюджета
        /// </summary>
        public DbSet<s_lev> s_lev { get; set; }

        /// <summary>
        /// Справочник кодов БК с процентами разбивки
        /// </summary>
        public DbSet<s_kbk> s_kbk { get; set; }

        /// <summary>
        /// Справочник дополнительных разбивок по процентам
        /// </summary>
        public DbSet<s_kbkdop> s_kbkdop { get; set; }

        /// <summary>
        /// Справочник кодов цели
        /// </summary>
        public DbSet<s_kceli> s_kceli { get; set; }

        /// <summary>
        /// Справочник ОКВЕД
        /// </summary>
        /// 
        public DbSet<s_bank> s_bank { get; set; }
        /// <summary>
        /// Справочник ОКВЕД
        /// </summary>
        /// 
        public DbSet<s_okved> s_okved { get; set; }

        /// <summary>
        /// Справочник плательщиков
        /// </summary>
        public DbSet<s_org> s_org { get; set; }

        /// <summary>
        /// Справочник плательщиков
        /// </summary>
        public DbSet<s_orgokved> s_orgokved { get; set; }

        /// <summary>
        /// Справочник счетов
        /// </summary>
        public DbSet<s_schet> s_schet { get; set; }


        /// <summary>
        /// Данные
        /// </summary>
        public DbSet<t_dohod> t_dohod { get; set; }

        /// <summary>
        /// Исходные данные 
        /// </summary>
        public DbSet<t_dohorig> t_dohorig { get; set; }

       /// <summary>
        /// Уведомления
        /// </summary>
        public DbSet<t_uv> t_uv { get; set; }

        /// <summary>
        /// Уведомления детали
        /// </summary>
        public DbSet<t_uvdet> t_uvdet { get; set; }

        /// <summary>
        /// Строуктурный уровень администраторов
        /// </summary>
        public DbSet<t_levelorg> t_levelorg { get; set; }

        /// <summary>
        /// Список форматов
        /// </summary>
        public DbSet<fk_doc> fk_doc { get; set; }

        /// <summary>
        /// Данные по формату
        /// </summary>
        public DbSet<fk_code> fk_code { get; set; }

        public DbSet<userrole> userrole { get; set; }
        public DbSet<User> user { get; set; }

        public DbSet<TypeUser> typeuser { get; set; }
        public DbSet<AccessForm> accessForm { get; set; }
        public DbSet<TypeForm> typeForm { get; set; }
        public DbSet<FormModel> formModel { get; set; }
        public DbSet<ButtonForm> buttonForm { get; set; }

        public DbSet<UserLinkRole> userlinkrole { get;set;}
        
        public DohodDbContext(IDataBaseMain dataBaseMain) : base(dataBaseMain)
        {
        }
    }
}
