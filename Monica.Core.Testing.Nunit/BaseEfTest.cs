using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.ModelCrm;
using Moq;
using NUnit.Framework;

namespace Module.Testing.NUnit
{
    public class BaseEfTest
    {
        private ReportDbContext _crmDbContext;
        public BaseEfTest()
        {
            var mock = new Mock<IDataBaseMain>();
            mock.Setup(main => main.ConntectionString).Returns(
                "Server=localhost;Port=3306;Database=monicacrm;User Id=RassvetAis;Password=RassvetLine6642965;TreatTinyAsBoolean=true;");
            _crmDbContext = new ReportDbContext(mock.Object);
        }

        [Test]
        public void GetUserInfo()
        {
            //var user = _crmDbContext.User.Include(u => u.TypeUser).Include(u => u.Manager).
            //    FirstOrDefault(f => f.Account == "UserTest");
            //var ss = _crmDbContext.TypeUser.FirstOrDefault();
        }
    }
}
