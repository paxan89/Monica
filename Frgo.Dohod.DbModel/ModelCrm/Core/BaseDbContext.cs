using Microsoft.EntityFrameworkCore;
using Monica.Core.DataBaseUtils;

namespace Frgo.Dohod.DbModel.ModelCrm.Core
{
    public class BaseDbContext : DbContext
    {
        private IDataBaseMain _dataBaseMain;

        public BaseDbContext(IDataBaseMain dataBaseMain)
        {
            _dataBaseMain = dataBaseMain;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_dataBaseMain.ConntectionString);
        }
    }
}
