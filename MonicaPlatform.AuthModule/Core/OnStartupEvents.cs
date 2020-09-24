using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monica.Core.Events;
using MonicaPlatform.AuthModule.Middleware;

namespace MonicaPlatform.AuthModule.Core
{
    /// <summary>
    /// Модуль включает или отключает режим авторизации, путем добавления
    /// глобального фильтра AuthorizeFilter
    /// </summary>
    public class OnStartupEvents : IOnStartupEvents
    {
        public void OnConfigureBefore(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory)
        {
            app.UseMiddleware<AuthMiddleware>();
        }

        public void OnConfigureAfter(IApplicationBuilder applicationBuilder, IWebHostEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory)
        {
        }

        public void OnConfigureServicesAfterAddMvc(IServiceCollection services, IMvcBuilder mvcBuilder,
            IConfiguration configuration = null)
        {

            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            mvcBuilder.AddMvcOptions(opt => opt.Filters.Add(new AuthorizeFilter(policy)));
        }
        
        public void OnConfigureServicesBeforeAddMvc(IServiceCollection services)
        {
        }

        public void OnInitBackendService(IServiceCollection services)
        {
        }

        public void OnConfigureBeforeUseMvc(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory)
        {
        }

        public void OnConfigureAfterUseMvc(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory iLoggerFactory)
        {
        }
    }
}
