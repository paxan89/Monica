using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MonicaPlatform.TechLog.Module.ActionFilters.Tools;

namespace MonicaPlatform.TechLog.Module.Middleware.RequestId
{
    /// <summary>
    /// Компонент для маркировки входящих запросов
    /// </summary>
    class RequestIdMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = context.TraceIdentifier;

            context.Items.Add(FilterConstants.RequestId, requestId);
            context.Response.Headers.Add(FilterConstants.RequestId, requestId);

            await _next.Invoke(context);
        }
    }
}
