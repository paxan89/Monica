using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MonicaPlatform.TechLog.Module.StructLogging
{
    /// <summary>
    /// Базовая модель для структурированного лога
    /// </summary>
    [Serializable]
    public abstract class BaseStructuredLogMessage : ISerializable
    {
        /// <summary>
        /// Пользователь, выполняющий вызов функции
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Имя хоста приложения
        /// </summary>
        public string HostName { get; set; } = (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 
                                                Environment.GetEnvironmentVariable("COMPUTERNAME") :
                                                Environment.GetEnvironmentVariable("HOSTNAME"));

        /// <summary>
        /// Уровень лога (Trace, Error, Access)
        /// </summary>
        public string LogLevel { get; set; }
        
        /// <summary>
        /// Длительность выполнения запроса
        /// </summary>
        public long DurationMilliseconds { get; set; }

        /// <summary>
        /// Дата и время создания записи
        /// </summary>
        public DateTime CreateDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Идентификатор запроса для связывания логируемых данных
        /// </summary>
        public string RequestId { get; set; }

        protected BaseStructuredLogMessage()
        {
        }

        #region Implementation of ISerializable

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("userName", UserName);
            info.AddValue("hostName", HostName);
            info.AddValue("logLevel", LogLevel);
            info.AddValue("duration", DurationMilliseconds);
            info.AddValue("createDateTime", CreateDateTime);
            info.AddValue("requestId", RequestId);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected BaseStructuredLogMessage(SerializationInfo info, StreamingContext context)
        {
            UserName = info.GetString("userName");
            HostName = info.GetString("hostName");
            LogLevel = info.GetString("logLevel");
            DurationMilliseconds = info.GetInt64("duration");
            CreateDateTime = info.GetDateTime("createDateTime");
            RequestId = info.GetString("requestId");
        }

        #endregion
    }
}
