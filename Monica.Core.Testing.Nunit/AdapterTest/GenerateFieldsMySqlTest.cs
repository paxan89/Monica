using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.Extension;
using Monica.Core.Service.ReportEngine;
using Monica.Core.Utils;
using Monica.CrmDbModel;
using Moq;
using NUnit.Framework;

namespace Module.Testing.Nunit.AdapterTest
{
    public class GenerateFieldsMySqlTest
    {
        protected IGenerateField Service;

        public GenerateFieldsMySqlTest()
        {
            var mockDataBaseMain = new Mock<IDataBaseMain>();
            var mockDataBaseIs4 = new Mock<IDataBaseIs4>();
            var logger = new Mock<ILogger>();

            var configiguration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            var connectionString = configiguration.ConnectionStrings.ConnectionStrings["MySqlDatabase"].ConnectionString;
            var connectionStringIs4 = configiguration.ConnectionStrings.ConnectionStrings["MySqlDatabaseIS4"].ConnectionString;

            mockDataBaseMain.Setup(main => main.ConntectionString).Returns(connectionString);
            mockDataBaseIs4.Setup(main => main.ConntectionString).Returns(connectionStringIs4);
            
            var services = new ServiceCollection();
            services.AddSingleton(mockDataBaseMain.Object);
            services.AddSingleton(mockDataBaseIs4.Object);
            services.AddDbContextCore();


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
                builder.Populate(services);
                builder.RegisterInstance(logger.Object);
                //builder.RegisterInstance(mockDataBaseIs4);
                //builder.RegisterInstance(mockDataBaseMain);

            });
            Service = AutoFac.ResolveNamed<IGenerateField>(nameof(GenerateFieldMySql));
        }


        [Test]
        public async Task GenerateField_Normal()
        {
            await Service.GenerateField(2);
            await Service.GenerateDefaultBtn(2);
            Assert.IsTrue(true);
        }
    }
}
