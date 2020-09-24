using Microsoft.AspNetCore.Builder;

namespace MonicaPlatform.TechLog.Module.Middleware.TechLog
{
    static class TechLogExtensions
    {
        public static IApplicationBuilder UseTechLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TechLogMiddleware>();
        }
    }
}
