using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Monica.Core.Exceptions;
using MonicaPlatform.TechLog.Module.StructLogging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MonicaPlatform.TechLog.Module.Middleware.TechLog
{
    /// <summary>
    /// Техническое логирование
    /// </summary>
    ///
    /// <remarks>
    /// Что делает данный компонент:
    /// 1 - Ставит в контекст текущего аутентифицированного пользователя;
    /// 2 - Создает сообщение для тех. логирования и ставит его в контекст запроса
    /// 3 - После отработки всех остальных компонентов, выполняет запись сообщения об ошибке,
    ///     если в контексте запроса будет обнаружено соответствующее значение - excpetionReceived;
    /// 4 - Запись итога работы запроса со всем его окружением в тех. лог.
    /// </remarks>
    class TechLogMiddleware
    {
        private readonly ILogger<TechLogMiddleware> _logger;
        private readonly RequestDelegate _next;

        public TechLogMiddleware(RequestDelegate next, ILogger<TechLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {

            var originalBody = context.Response.Body;

            try
            {
                //await FixUserName(context);

                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    var webApiLogMessage = LogTools.CreateLogMessage<WebApiLogMessage>(context);

                    await _next(context);

                    await FixException(context);

                    await FinalizeResponse(memStream, originalBody, context, _logger, webApiLogMessage);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }

        /// <summary>
        /// Фиксация исключения, и изменение ответа клиенту.
        /// </summary>
        private static async Task FixException(HttpContext context)
        {
            if (context.Items.ContainsKey("excpetionReceived"))
            {
                var excpetionReceived = context.Items["excpetionReceived"] as Exception;
                var userMessageException = excpetionReceived as UserMessageException;

                string errorMessage;

                if (userMessageException != null)
                {
                    errorMessage = userMessageException.Message;

                    context.Response.StatusCode = 400;
                }
                else
                {
                    errorMessage =
                        "Уважаемый пользователь, на сервере возникла ошибка, обратитесь, пожалуйста, в службу технической поддержки.";

                    context.Response.StatusCode = 500;
                }

                var json = new JObject();
                json["status"] = context.Response.StatusCode;
                json["type"] = "https://tools.ietf.org/html/rfc7235#section-3.1";
                json["title"] = "Bad Request";
                json["traceId"] = context.TraceIdentifier;
                json["detail"] = errorMessage;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(json));
                
                context.Response.ContentType = "text/html;charset=utf-8";
            }
        }

        //private async Task FixUserName(HttpContext context)
        //{
        //    if (!context.Items.ContainsKey(FilterConstants.UserName) &&
        //        context.User?.Identity != null &&
        //        !string.IsNullOrEmpty(context.User.Identity.Name))
        //    {
        //        context.Items.Add(FilterConstants.UserName, context.User.Identity.Name);
        //    }
        //    //else
        //    //{
        //    //    var authResult = await context.AuthenticateAsync("Bearer");
        //    //    if (!context.Items.ContainsKey(FilterConstants.UserName) && !string.IsNullOrEmpty(authResult?.Principal?.Identity?.Name))
        //    //        context.Items.Add(FilterConstants.UserName, authResult?.Principal?.Identity?.Name);
        //    //}
        //}

        /// <summary>
        /// Финализация обработки запроса
        /// </summary>
        private async Task FinalizeResponse(MemoryStream memStream,
            Stream originalBody,
            HttpContext context,
            ILogger logger,
            WebApiLogMessage webApiLogMessage)
        {
            memStream.Position = 0;
            var responceBodyStream = new StreamReader(memStream);
            using (responceBodyStream)
            {
                string responseBody = responceBodyStream.ReadToEnd();

                memStream.Position = 0;
                await memStream.CopyToAsync(originalBody);

                if (context.Items.ContainsKey("excpetionReceived"))
                {
                    var webApiExceptionLogMessage = LogTools.CreateLogMessage<WebApiExceptionLogMessage>(context);
                    webApiExceptionLogMessage.ExceptionReceived = context.Items["excpetionReceived"] as Exception;

                    LogTools.WriteLogMessage(context, webApiExceptionLogMessage, logger, responseBody);
                }
                else
                {
                    LogTools.WriteLogMessage(context, webApiLogMessage, logger, responseBody);
                }
            }
        }

    }
}
