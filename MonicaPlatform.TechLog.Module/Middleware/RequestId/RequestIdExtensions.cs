using Microsoft.AspNetCore.Builder;

namespace MonicaPlatform.TechLog.Module.Middleware.RequestId
{
    static class RequestIdExtensions
    {
        public static IApplicationBuilder UseRequestId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestIdMiddleware>();
        }
    }
}
