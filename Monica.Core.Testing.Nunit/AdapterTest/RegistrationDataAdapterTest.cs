using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Monica.Core.Abstraction.Registration;
using Monica.Core.DbModel.IdentityModel;
using Monica.Core.ModelParametrs.ModelsArgs;
using Monica.Core.Utils;
using NUnit.Framework;

namespace Module.Testing.Nunit.AdapterTest
{
    public class RegistrationDataAdapterTest : BaseServiceTest<IRegistrationUserAdapter>
    {
        [Test]
        public async Task RegistrationUserTest_Normal()
        {
            bool isRegisterUser;
            try
            {
                await Service.RemoveUser("TestUser");
                await Service.Register(new RegistrationUserArgs
                {
                    Account = "TestUser",
                    Email = "120789p@gmail.com",
                    Surname = "Test",
                    Middlename = "Test",
                    Name = "Test",
                    Password = "P@ssword12"
                });
                isRegisterUser = true;
            }
            finally
            {
                await Service.RemoveUser("TestUser");
            }

            Assert.IsTrue(isRegisterUser);
        }

        [Test]
        public async Task RegistrationUserAndSetPasswordTest_Normal()
        {
            bool isRegisterUser;
            try
            {
                await Service.RemoveUser("TestUser");
                await Service.RegisterAndSetPassword(new RegistrationUserArgs
                {
                    Account = "TestUser",
                    Email = "120789p@gmail.com",
                    Surname = "Test",
                    Middlename = "Test",
                    Name = "Test",
                    Password = "zXcvbnm512%"
                });
                isRegisterUser = true;
            }
            finally
            {
                await Service.RemoveUser("TestUser");
            }

            Assert.IsTrue(isRegisterUser);
        }

        [Test]
        public async Task GetRoles()
        {
            var roleManager = AutoFac.Resolve<RoleManager<ApplicationRole>>();
            var ss = roleManager.Roles.Select(s => s.Name);
            Assert.IsNotEmpty(ss);
        }
    }
}
