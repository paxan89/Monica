using System;
using System.Diagnostics;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monica.Core.Constants;
using Monica.Core.DataBaseUtils;
using Monica.Core.Utils;
using Monica.PlatformMain.LoaderModules;
using LoaderExtensions = Monica.PlatformMain.LoaderModules.LoaderExtensions;

namespace Monica.PlatformMain
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IContainer ApplicationContainer { get; private set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            LoaderExtensions.OnConfigureServicesBeforeAddMvc(services);
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("*")
                        .WithMethods("*")
                        .WithHeaders("*")
                        .DisallowCredentials();
                });
            });
            IMvcBuilder mvcBuilder = MvcServiceCollectionExtensions.AddMvc(services).AddNewtonsoftJson();
            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new AuthorizeFilter("CorsPolicyAll"));
            //});
            services.AddControllers(mvcOtions =>
            {
                mvcOtions.EnableEndpointRouting = false;
            });
            mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddScoped<IDataBaseMain, DataBaseMain>();
            services.AddScoped<IDataBaseIs4, DataBaseIs4>();
            LoaderExtensions.OnConfigureServicesAfterAddMvc(services, mvcBuilder,Configuration);
            LoaderExtensions.LoadMvc(mvcBuilder, GlobalSettingsApp.CurrentAppDirectory);
            LoaderExtensions.OnInitBackendService(services);

            ApplicationContainer = AutoFac.Init(DataBaseName.MySql, cb =>
            {
                cb.Populate(services);
            });
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            ILoggerFactory iLoggerFactory)
        {

            LoaderExtensions.OnConfigureBefore(app, env, iLoggerFactory);
            LoaderExtensions.OnConfigureBeforeUseMvc(app, env, iLoggerFactory);


            if (Debugger.IsAttached)
                app.UseCors(builder => builder.WithOrigins("https://localhost:8443")
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseMvc();
            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            LoaderExtensions.OnConfigureAfterUseMvc(app, env, iLoggerFactory);
            LoaderExtensions.OnConfigureAfter(app, env, iLoggerFactory);
        }
    }
}
