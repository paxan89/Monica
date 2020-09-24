using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MonicaPlatform.TechLog.Module.StructLogging;
using Newtonsoft.Json;

namespace MonicaPlatform.TechLog.Module.ActionFilters.Tools
{
    /// <summary>
    /// Вспомогательный класс для работы с фильтрами ASP.NET CORE
    /// </summary>
    public static class FilterTools
    {
        /// <summary>
        /// Установить длительность выполнения запроса
        /// </summary>
        /// 
        /// <param name="context">Контекст исполнения запроса</param>
        /// <param name="logMessage">Модель структурированного сообщения для логирования</param>
        public static void SetDuration(FilterContext context, WebApiLogMessage logMessage)
        {
            if (!context.HttpContext.Items.TryGetValue(FilterConstants.Stopwatch, out var stopWatchObject))
            {
                return;
            }

            if (!(stopWatchObject is Stopwatch stopwatch))
            {
                return;
            }

            stopwatch.Stop();
            logMessage.DurationMilliseconds = stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Записать в лог статус ответа Action функции клиенту
        /// </summary>
        /// 
        /// <param name="context">Контекст исполнения запроса</param>
        /// <param name="logMessage">Модель структурированного сообщения для логирования</param>
        public static void SetStatusCode(ResultExecutedContext context, WebApiLogMessage logMessage)
        {
            if (context.Result == null)
            {
                return;
            }

            var property = context.Result.GetType().GetProperty("StatusCode");
            if (property == null)
            {
                return;
            }

            var value = property.GetValue(context.Result);
            if (int.TryParse(value.ToString(), out var code))
            {
                logMessage.StatusCode = code;
            }
        }

        /// <summary>
        /// Записать в лог статус ответа Action функции клиенту
        /// </summary>
        /// 
        /// <param name="context">Контекст исполнения запроса</param>
        /// <param name="logMessage">Модель структурированного сообщения для логирования</param>
        public static void SetStatusCode(ExceptionContext context, WebApiLogMessage logMessage)
        {
            if (context.Result == null)
            {
                return;
            }

            var property = context.Result.GetType().GetProperty("StatusCode");
            if (property == null)
            {
                return;
            }

            var value = property.GetValue(context.Result);
            
            if (int.TryParse(value.ToString(), out var code))
            {
                logMessage.StatusCode = code;
            }
        }

        /// <summary>
        /// Записать в лог ответ Action метода контроллера клиенту
        /// </summary>
        /// 
        /// <param name="context">Контекст исполнения запроса</param>
        /// <param name="logMessage">Модель структурированного сообщения для логирования</param>
        /// <param name="logger"></param>
        public static void SetResponce(ResultExecutedContext context, WebApiLogMessage logMessage, ILogger logger)
        {
            if (logMessage.StatusCode > 399 || logger.IsEnabled(LogLevel.Debug))
            {
                logMessage.HttpResponce = JsonConvert.SerializeObject(context.Result);
            }
        }

        /// <summary>
        /// Записать в лог ответ Action метода контроллера клиенту
        /// </summary>
        /// 
        /// <param name="context">Контекст исполнения запроса</param>
        /// <param name="logMessage">Модель структурированного сообщения для логирования</param>
        /// <param name="logger"></param>
        public static void SetResponce(ExceptionContext context, WebApiLogMessage logMessage, ILogger logger)
        {
            if (logMessage.StatusCode > 399 || logger.IsEnabled(LogLevel.Debug))
            {
                logMessage.HttpResponce = JsonConvert.SerializeObject(context.Result);
            }
        }

        /// <summary>
        /// Записать в лог текущего пользователя
        /// </summary>
        /// 
        /// <param name="context">Контекст исполнения запроса</param>
        /// <param name="logMessage">Модель структурированного сообщения для логирования</param>
        public static void SetUserName(FilterContext context, WebApiLogMessage logMessage)
        {
            if (context.HttpContext.Items.ContainsKey(FilterConstants.UserName))
            {
                logMessage.UserName = context.HttpContext.Items[FilterConstants.UserName]?.ToString();
            }
        }

        /// <summary>
        /// Записать идентификатор запроса
        /// </summary>
        /// 
        /// <param name="context">Контекст исполнения запроса</param>
        /// <param name="logMessage">Модель структурированного сообщения для логирования</param>
        public static void SetRequestId(FilterContext context, WebApiLogMessage logMessage)
        {
            if (context.HttpContext.Items.ContainsKey(FilterConstants.RequestId))
            {
                logMessage.RequestId = context.HttpContext.Items[FilterConstants.RequestId].ToString();
            }
        }
    }
}
