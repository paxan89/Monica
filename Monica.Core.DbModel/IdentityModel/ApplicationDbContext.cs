using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Monica.Core.DataBaseUtils;

namespace Monica.Core.DbModel.IdentityModel
{
    /// <summary>
    /// Конекст данных для работы с БД IS4
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>, IPersistedGrantDbContext
    {
        private IDataBaseIs4 _dataBaseMain;

        public ApplicationDbContext(IDataBaseIs4 dataBaseMain)
        {
            _dataBaseMain = dataBaseMain;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_dataBaseMain.ConntectionString);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }
    }


}
