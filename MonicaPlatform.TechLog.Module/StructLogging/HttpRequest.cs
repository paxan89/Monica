using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace MonicaPlatform.TechLog.Module.StructLogging
{
    /// <summary>
    /// Модель входящего запроса
    /// </summary>
    [Serializable]
    public class HttpRequest : ISerializable
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// 
        /// <param name="context">Контекст фильтра</param>
        public HttpRequest(HttpContext context)
        {
            var httpRequest = context.Request;

            if (context.Items.ContainsKey("requestBody"))
            {
                RequestBody = context.Items["requestBody"].ToString();
            }

            Method = httpRequest.Method;
            Query = httpRequest.GetDisplayUrl();
        }

        /// <summary>
        /// Входящая ссылка
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Метод запроса
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Тело запроса
        /// </summary>
        public string RequestBody { get; set; }

        #region Implementation of ISerializable

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private HttpRequest(SerializationInfo info, StreamingContext context)
        {
            Query = info.GetString("query");
            Method = info.GetString("method");
            RequestBody = info.GetString("requestBody");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("query", Query);
            info.AddValue("method", Method);
            info.AddValue("requestBody", RequestBody);
        }

        #endregion
    }
}
