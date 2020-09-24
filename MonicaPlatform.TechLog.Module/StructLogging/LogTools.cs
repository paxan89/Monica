using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Monica.Core.Constants;
using MonicaPlatform.TechLog.Module.ActionFilters.Tools;
using Newtonsoft.Json;

namespace MonicaPlatform.TechLog.Module.StructLogging
{
    /// <summary>
    /// Вспомогательный функционал для записи в лог
    /// </summary>
    public static class LogTools
    {

        public static TLogMessage CreateLogMessage<TLogMessage>(HttpContext context)
            where TLogMessage : WebApiLogMessage, new()
        {
            var webApiLogMessage = new TLogMessage
            {
                RequestId = context.TraceIdentifier,
                LogLevel = "WebApi.Info",
                HttpRequest = new HttpRequest(context)
            };

            var queryItems = context.Request.Path.Value.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (queryItems.Length == 2)
            {
                webApiLogMessage.ControllerName = queryItems[0];
                webApiLogMessage.ActionName = queryItems[1];
            }

            if (!context.Items.ContainsKey(FilterConstants.WebApiLogMessage))
            {
                context.Items.Add(FilterConstants.WebApiLogMessage, webApiLogMessage);
            }

            return webApiLogMessage;
        }

        public static void WriteLogMessage(HttpContext context,
            WebApiLogMessage webApiLogMessage,
            ILogger logger,
            string responseBody)
        {
            webApiLogMessage.StatusCode = context.Response.StatusCode;
            if (context.Response.StatusCode > 399 || logger.IsEnabled(LogLevel.Debug))
            {
                webApiLogMessage.HttpResponce = responseBody;
            }

            SetDuration(context, webApiLogMessage);

            if (context.Items.ContainsKey(FilterConstant.UserName))
            {
                webApiLogMessage.UserName = context.Items[FilterConstant.UserName]?.ToString();
            }

            try
            {
                if (webApiLogMessage.StatusCode < 400)
                    logger.LogInformation("RequestId:{RequestId}\r\nActionName:{ActionName}\r\nControllerName:{ControllerName}\r\nStatusCode:{StatusCode}\r\nUserName:{UserName}\r\nDurationMilliseconds:{DurationMilliseconds}\r\nHttpResponce:{HttpResponce}", new object[]
                    {
                    webApiLogMessage.RequestId,
                    webApiLogMessage.ActionName,
                    webApiLogMessage.ControllerName,
                    webApiLogMessage.StatusCode,
                    webApiLogMessage.UserName,
                    webApiLogMessage.DurationMilliseconds,
                    webApiLogMessage.HttpResponce
                    });
                else logger.LogError("RequestId:{RequestId}\r\nActionName:{ActionName}\r\nControllerName:{ControllerName}\r\nStatusCode:{StatusCode}\r\nUserName:{UserName}\r\nDurationMilliseconds:{DurationMilliseconds}\r\nHttpResponce:{HttpResponce}\r\nErrorMessage:{ErrorMessage}", new object[]
                {
                    webApiLogMessage.RequestId,
                    webApiLogMessage.ActionName,
                    webApiLogMessage.ControllerName,
                    webApiLogMessage.StatusCode,
                    webApiLogMessage.UserName,
                    webApiLogMessage.DurationMilliseconds,
                    webApiLogMessage.HttpResponce,
                    context.Items.ContainsKey("excpetionReceived") ? context.Items["excpetionReceived"] : null
                });
            }
            catch (JsonSerializationException)
            {
                if (context.Items.ContainsKey("excpetionReceived"))
                {
                    if (webApiLogMessage is WebApiExceptionLogMessage msg)
                    {
                        msg.ExceptionText = msg.ExceptionReceived.ToString();
                        msg.ExceptionReceived = null;
                        logger.LogInformation(msg.ExceptionText, webApiLogMessage);
                    }

                }
            }
        }

        /// <summary>
        /// Сохранить длительность выполнения запроса
        /// </summary>
        /// 
        /// <param name="context">Контекст исполнения запроса</param>
        /// <param name="logMessage">Модель структурированного сообщения для логирования</param>
        public static void SetDuration(HttpContext context, WebApiLogMessage logMessage)
        {
            if (!context.Items.TryGetValue(FilterConstants.Stopwatch, out var stopWatchObject))
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
    }
}
