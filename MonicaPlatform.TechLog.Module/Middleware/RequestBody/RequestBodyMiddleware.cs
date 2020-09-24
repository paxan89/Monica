using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MonicaPlatform.TechLog.Module.Middleware.RequestBody
{
    /// <summary>
    /// Промежуточный компонент выполняет размещение тела запроса в
    /// контексте с ключем "requestBody", для анализа другими компонентами
    /// </summary>
    class RequestBodyMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly Dictionary<string, bool> _ignoresDictionary = new Dictionary<string, bool>()
        {
            { "/tablereports/setdata", true }
        };

        public RequestBodyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var requestBody = "";

                _ignoresDictionary.TryGetValue(context.Request.Path.Value.ToLowerInvariant(), out var ignoreBody);

                if (!(context.Request.ContentLength > 0) || ignoreBody)
                {
                    context.Items.Add("requestBody", requestBody);
                    await _next.Invoke(context);
                    return;
                }
                
                // фиксируем форму если она есть
                if (FixRequestForm(context))
                {
                    await _next.Invoke(context);
                    return;
                }

                // проверяем не содержит ли запрос бинарных данных, так как их проверять не нужно
                if (!IsTextContentType(context))
                {
                    context.Items.Add("requestBody", requestBody);
                    await _next.Invoke(context);
                    return;
                }

                context.Request.EnableBuffering();
                var stream = context.Request.Body;
        
                using (var reader = new StreamReader(stream))
                {
                    requestBody = await reader.ReadToEndAsync();

                    if (stream.CanSeek)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }

                    context.Items.Add("requestBody", requestBody);

                    await _next.Invoke(context);
                }
            }
            catch (Exception exception)
            {
                var errorMessage = $"Ошибка записи параметров запроса: {exception}";

                Console.WriteLine(errorMessage);

                if (context.Items.ContainsKey("requestBody"))
                {
                    context.Items["requestBody"] = errorMessage;
                }
                else
                {
                    context.Items.Add("requestBody", errorMessage);
                }
                
                await _next.Invoke(context);
            }
            
        }

        private static bool IsTextContentType(HttpContext context)
        {
            var listTypes = new List<string> { "json", "text", "xml", "xhtml" };

            foreach (var type in listTypes)
            {
                var result = context.Request.ContentType.Contains(type);
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Фиксация формы для дальнейшей обработки
        /// </summary>
        private bool FixRequestForm(HttpContext context)
        {
            if (!context.Request.HasFormContentType)
            {
                return false;
            }

            if (context.Request.Form != null)
            {
                var requestBody = JsonConvert.SerializeObject(context.Request.Form);

                context.Items.Add("requestBody", requestBody);
            }

            return true;

        }
    }
}
