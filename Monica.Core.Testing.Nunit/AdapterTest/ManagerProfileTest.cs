using System.Linq;
using System.Threading.Tasks;
using Monica.Core.Abstraction.Profile;
using Monica.Core.ModelParametrs.ModelsArgs;
using NUnit.Framework;

namespace Module.Testing.Nunit.AdapterTest
{
    public class ManagerProfileTest : BaseServiceTest<IManagerProfile>
    {
        [Test]
        public async Task GetRoles_Normal()
        {
            var res = await Service.GetRoleUsers("Administrator");
            Assert.IsTrue(res.Any());
        }

        [Test]
        public async Task AddToRoleByUser_Normal()
        {
            var res = await Service.AddToRoleByUser("Administrator", "FunctionalAdministrator");
        }


        [Test]
        public async Task GetUsers_Normal()
        {
            var res = await Service.GetListUsers(new UserArgs {FullName = "Admin", Page = 1, PageSize = 10});
            Assert.IsTrue(res.Results.Any());
        }
    }
}
