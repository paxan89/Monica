using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MonicaPlatform.TechLog.Module.StructLogging
{
    public class WebApiLogMessage : BaseStructuredLogMessage
    {
        /// <summary>
        /// Наименование контроллера
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Наименование действия на контроллере
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Описание параметров входящего запроса
        /// </summary>
        public HttpRequest HttpRequest { get; set; }

        /// <summary>
        /// Статус обработки запроса
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Ответ сервера клиенту
        /// </summary>
        public string HttpResponce { get; set; }
        
        public WebApiLogMessage() { }

        #region ISerializable

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private WebApiLogMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ControllerName = info.GetString("controllerName");
            ActionName = info.GetString("actionName");
            HttpRequest = info.GetValue("httpRequest", typeof(HttpRequest)) as HttpRequest;
            StatusCode = info.GetInt32("httpStatusCode");
            HttpResponce = info.GetString("httpResponce");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("controllerName", ControllerName);
            info.AddValue("actionName", ActionName);
            info.AddValue("httpRequest", HttpRequest);
            info.AddValue("httpStatusCode", StatusCode);
            info.AddValue("httpResponce", HttpResponce);
        }

        #endregion
    }
}
