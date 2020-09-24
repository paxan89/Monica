using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monica.Core.Events;
using MonicaPlatform.TechLog.Module.ActionFilters;
using MonicaPlatform.TechLog.Module.Core;
using MonicaPlatform.TechLog.Module.Middleware.RequestBody;
using MonicaPlatform.TechLog.Module.Middleware.RequestId;
using MonicaPlatform.TechLog.Module.Middleware.StopWatch;
using MonicaPlatform.TechLog.Module.Middleware.TechLog;

namespace MonicaPlatform.TechLog.Module
{
    public class OnStartupEvents : IOnStartupEvents
    {
        #region Implementation of IOnStartupEvents

        public void OnConfigureBefore(IApplicationBuilder applicationBuilder, IWebHostEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory)
        {
            //Измерение времени выполнения запроса
            applicationBuilder.UseStopwatch();

            //Маркировка входящих запросов
            applicationBuilder.UseRequestId();

            //фиксация тела запроса, для анализа другими компонентами
            applicationBuilder.UseRequestBody();

            //выполнение технического логирования
            applicationBuilder.UseTechLog();

        }

        public void OnConfigureAfter(IApplicationBuilder applicationBuilder, IWebHostEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory)
        {
        }

        public void OnConfigureServicesAfterAddMvc(IServiceCollection services, IMvcBuilder mvcBuilder,
            IConfiguration configuration = null)
        {
            mvcBuilder.AddMvcOptions(s => { s.Filters.Add<ExceptionFilter>(); });
        }
        
        public void OnConfigureServicesBeforeAddMvc(IServiceCollection services)
        {
            
        }

        public void OnConfigureServicesBeforeAddMvc(IServiceCollection services, IMvcBuilder mvcBuilder)
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

        #endregion
    }
}
