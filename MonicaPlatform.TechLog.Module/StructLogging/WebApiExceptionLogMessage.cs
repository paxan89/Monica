using System;
using System.Runtime.Serialization;

namespace MonicaPlatform.TechLog.Module.StructLogging
{
    /// <summary>
    /// Модель сообщения для возникающих исключений в WebApi
    /// </summary>
    public class WebApiExceptionLogMessage : WebApiLogMessage
    {
        /// <summary>
        /// Полученное исключение
        /// </summary>
        public Exception ExceptionReceived { get; set;}

        /// <summary>
        /// Текст полученного исключения, если не удалось сериализовать само исключение.
        /// Такое может быть при возникновении исключения с циклическим ссылками в свойства TargetSite.
        /// </summary>
        public string ExceptionText { get; set; }

        public WebApiExceptionLogMessage()
        {

        }

        #region ISerializable
        private WebApiExceptionLogMessage(SerializationInfo info, StreamingContext context)
        {
            ExceptionReceived = info.GetValue("exceptionReceived", typeof(Exception)) as Exception;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (ExceptionReceived != null)
            {
                info.AddValue("exceptionReceived", ExceptionReceived);
            }

            if(!string.IsNullOrEmpty(ExceptionText))
            {
                info.AddValue("exceptionReceivedText", ExceptionText);
            }
        }
        
        #endregion
    }
}
