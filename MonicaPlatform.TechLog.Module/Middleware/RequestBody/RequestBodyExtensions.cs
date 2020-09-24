using Microsoft.AspNetCore.Builder;

namespace MonicaPlatform.TechLog.Module.Middleware.RequestBody
{
    static class RequestBodyExtensions
    {
        public static IApplicationBuilder UseRequestBody(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestBodyMiddleware>();
        }
    }
}
