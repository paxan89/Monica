using Frgo.Dohod.DbModel.ModelCrm;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monica.Core.Events;

namespace Frgo.Dohod.DbModel
{
    public class OnStartupEvents : IOnStartupEvents
    {
        public void OnConfigureBefore(IApplicationBuilder applicationBuilder, IWebHostEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory) { }

        public void OnConfigureAfter(IApplicationBuilder applicationBuilder, IWebHostEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory) { }

        public void OnConfigureServicesAfterAddMvc(IServiceCollection services, IMvcBuilder mvcBuilder,
            IConfiguration configuration = null)
        {
            services.AddDbContext<DohodDbContext>(ServiceLifetime.Transient);
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
