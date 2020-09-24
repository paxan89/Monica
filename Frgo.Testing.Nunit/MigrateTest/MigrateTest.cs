using Frgo.Dohod.DbModel.ModelCrm.Core;
using Monica.CrmDbModel.Extension;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.DataAdapter.Migrate;

namespace Module.Testing.Nunit.MigrateTest
{


    public class MigrateDbTest : BaseServiceTest<IMigrateFoxToMySql>
    {
         public MigrateDbTest() : base()
        {
            var enc = CodePagesEncodingProvider.Instance.GetEncoding(1251);
        }

        [Test]
        public async Task MigrateFromFox_Normal()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var dirFoxDb = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FoxDataBase");
            await Service.MigrateFromFox(dirFoxDb, CodePagesEncodingProvider.Instance.GetEncoding(1251));
            var msmgrate = stopwatch.Elapsed.TotalMilliseconds;
            Assert.IsTrue(true);
        }

    }
}
