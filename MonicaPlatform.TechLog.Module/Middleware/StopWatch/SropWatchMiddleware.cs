using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MonicaPlatform.TechLog.Module.ActionFilters.Tools;

namespace MonicaPlatform.TechLog.Module.Middleware.StopWatch
{
    /// <summary>
    /// Компонент для измерения времени выполнения запроса
    /// </summary>
    class SropWatchMiddleware
    {
        private readonly RequestDelegate _next;

        public SropWatchMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            context.Items.Add(FilterConstants.Stopwatch, stopwatch);

            await _next.Invoke(context);
        }
    }
}
