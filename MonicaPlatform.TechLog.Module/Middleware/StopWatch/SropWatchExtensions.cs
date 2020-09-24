using Microsoft.AspNetCore.Builder;

namespace MonicaPlatform.TechLog.Module.Middleware.StopWatch
{
    static class SropWatchExtensions
    {
        public static IApplicationBuilder UseStopwatch(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SropWatchMiddleware>();
        }
    }
}
