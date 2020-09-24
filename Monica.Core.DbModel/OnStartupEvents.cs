using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monica.Core.DbModel.IdentityModel;
using Monica.Core.DbModel.Localization.Identity;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.Events;

namespace Monica.Core.DbModel
{
    public class OnStartupEvents : IOnStartupEvents
    {
        public void OnConfigureBefore(IApplicationBuilder applicationBuilder, IWebHostEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory) { }

        public void OnConfigureAfter(IApplicationBuilder applicationBuilder, IWebHostEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory) { }

        public void OnConfigureServicesAfterAddMvc(IServiceCollection services, IMvcBuilder mvcBuilder, IConfiguration configuration = null)
        {
            services.AddDbContext<ApplicationDbContext>();
            services.AddDbContext<ReportDbContext>(ServiceLifetime.Transient);
            services.AddIdentity<ApplicationUser, ApplicationRole>().
                AddErrorDescriber<RussiaIdentityErrorDescriber>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        /// <summary>
        /// Нужно зарешистрировать контекст для работы с БД.
        /// Возможно будет больше контекстов. Сейчас обязательно контекст для работы с БД IdentityServer4
        /// </summary>
        /// <param name="services"></param>
        public void OnConfigureServicesBeforeAddMvc(IServiceCollection services)
        {
        }

        public void OnInitBackendService(IServiceCollection services) { }

        public void OnConfigureBeforeUseMvc(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory) { }

        public void OnConfigureAfterUseMvc(IApplicationBuilder app, IWebHostEnvironment env,
            ILoggerFactory iLoggerFactory)
        {
        }
    }
}
