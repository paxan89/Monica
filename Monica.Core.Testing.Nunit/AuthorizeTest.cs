using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Monica.Core.Abstraction.Authorize;
using Monica.Core.DbModel.ModelsAuth;
using Monica.Core.Utils;
using NUnit.Framework;

namespace Module.Testing.Nunit
{
    /// <summary>
    /// Тест авторизации до IdentityServer4
    /// </summary>
    public class AuthorizeTest
    {
        private IMonicaAuthorizeDataAdapter _monicaAuthorizeDataAdapter;

        public AuthorizeTest()
        {
            var confFileName = Path.Combine(
                Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty,
                $"Monica.Crm.WebApi.dll.config");
            var build = new ConfigurationBuilder().AddXmlFile(confFileName);
            var configiguration = build.Build(); 
            
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            foreach (var file in files)
            {
                if (file.IndexOf("testhost.dll", StringComparison.Ordinal) > -1)
                {
                    continue;
                }

                System.Runtime.Loader.AssemblyLoadContext.Default
                    .LoadFromAssemblyPath(file);
            }

            AutoFac.Init(DataBaseName.MySql, builder =>
                {
                    builder.RegisterInstance(configiguration).As<IConfiguration>();
                });
            _monicaAuthorizeDataAdapter = AutoFac.Resolve<IMonicaAuthorizeDataAdapter>();

        }
        [Test]
        public async Task Login()
        {
            var token = await _monicaAuthorizeDataAdapter.LoginAsync(new UserAuthArgs
            {
                Login = "Administrator",
                Password = "Pass123$"
            });
            Assert.IsNotEmpty(token.AccessToken);
        }
        [Test]
        public async Task RefreshToken()
        {
            var token = await _monicaAuthorizeDataAdapter.RefreshTokenAsunc("8bf904bdcda0b03ab048e0c4a8ea0f24857546b3bdd0e8afd40858c27e5fc0ad");
            Assert.IsNotEmpty(token.RefreshToken);
        }

    }
}
