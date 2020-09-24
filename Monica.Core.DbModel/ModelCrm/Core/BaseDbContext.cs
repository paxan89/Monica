using Microsoft.EntityFrameworkCore;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.ModelCrm.Profile;

namespace Monica.Core.DbModel.ModelCrm.Core
{
    public class BaseDbContext : DbContext
    {
        private IDataBaseMain _dataBaseMain;

        public BaseDbContext(IDataBaseMain dataBaseMain)
        {
            _dataBaseMain = dataBaseMain;
        }
        /// <summary>
        /// Игроки, менеджеры
        /// </summary>
        public DbSet<User> User { get; set; }
        /// <summary>
        /// Пользовательские роли
        /// </summary>
        public DbSet<UserRole> UserRole { get; set; }
        /// <summary>
        /// Сызяь между пользователем и ролью
        /// </summary>
        public DbSet<UserLinkRole> UserLinkRole { get; set; }
        /// <summary>
        /// Тип пользователя
        /// </summary>
        public DbSet<TypeUser> TypeUser { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_dataBaseMain.ConntectionString);
        }
    }
}
