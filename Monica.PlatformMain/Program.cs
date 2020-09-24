using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NLog.Web;
using LoaderExtensions = Monica.PlatformMain.LoaderModules.LoaderExtensions;

namespace Monica.PlatformMain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoaderExtensions.Load(Path.GetDirectoryName(typeof(Program).Assembly.Location));

            LoaderExtensions.OnProgramMainBeforeRun(args);
            IWebHost iWebHost = CreateWebHostBuilder(args).Build();
            LoaderExtensions.OnProgramBuildWebHostAfterBuild(args, iWebHost);
            iWebHost.Run();
            LoaderExtensions.OnProgramMainAfterRun(args);
            NLog.LogManager.Shutdown();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var conf = builder.Build();
            LoaderExtensions.OnProgramBuildWebHostBeforeCreateDefaultBuilder(args);
            IWebHostBuilder defaultBuilder = WebHost.CreateDefaultBuilder(args);
            defaultBuilder.UseNLog();
            defaultBuilder.UseConfiguration(conf);
            LoaderExtensions.OnProgramBuildWebHostBeforeUseStartup(args, defaultBuilder);
            return defaultBuilder.UseStartup<Startup>();
        }
    }
}
