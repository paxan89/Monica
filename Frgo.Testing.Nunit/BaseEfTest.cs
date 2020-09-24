using Frgo.Dohod.DbModel.ModelCrm;
using Monica.Core.DataBaseUtils;
using Moq;
using NUnit.Framework;

namespace Module.Testing.NUnit
{
    public class BaseEfTest
    {
        private DohodDbContext _crmDbContext;
        public BaseEfTest()
        {
            var mock = new Mock<IDataBaseMain>();
            //mock.Setup(main => main.ConntectionString).Returns(
            //    "Server=localhost;Port=3306;Database=monicacrm;User Id=RassvetAis;Password=RassvetLine6642965;TreatTinyAsBoolean=true;");
            mock.Setup(main => main.ConntectionString).Returns(
                "Server=localhost;Port=3307;Database=aisdohoddevelop;User Id=RassvetAis;Password=RassvetLine6642965;TreatTinyAsBoolean=true;charset=utf8;");

      _crmDbContext = new DohodDbContext(mock.Object);
        }
    }
}
