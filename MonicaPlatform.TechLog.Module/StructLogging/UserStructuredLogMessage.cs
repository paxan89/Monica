using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MonicaPlatform.TechLog.Module.StructLogging
{
    /// <summary>
    /// Структурированное сообщение для записи пользовательского сообщения
    /// </summary>
    public class UserStructuredLogMessage : BaseStructuredLogMessage
    {
        /// <summary>
        /// Пользовательское сообщение в лог
        /// </summary>
        public string UserMessage { get; set; }

        internal UserStructuredLogMessage(string userMessage, string requestId)
        {
            UserMessage = userMessage;
            RequestId = requestId;
            LogLevel = "Trace.UserMaessage";
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("userMessage", UserMessage);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected UserStructuredLogMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            UserMessage = info.GetString("userMessage");
        }
    }
}
