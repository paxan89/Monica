using Newtonsoft.Json;

namespace MonicaPlatform.TechLog.Module.StructLogging
{
    /// <summary>
    /// Модель логируемого сообщения на запрос доступа
    /// </summary>
    /// 
    /// <typeparam name="TInAccessRequestModel">Входная модель запроса из сборки Platform.SystemAccess</typeparam>
    /// <typeparam name="TOutAccessRequestModel">Выходная модель запроса из сборки Platform.SystemAccess </typeparam>
    public sealed class AccessLogMessage<TInAccessRequestModel, TOutAccessRequestModel> : BaseStructuredLogMessage
    {
        /// <summary>
        /// Входной запрос
        /// </summary>
        [JsonProperty("accessRequest")]
        public TInAccessRequestModel AccessRequest { get; set; }

        /// <summary>
        /// Выходной запрос
        /// </summary>
        [JsonProperty("accessResponce")]
        public TOutAccessRequestModel AccessResponce { get; set; }

        /// <summary>
        /// Дополнительные параметры для логирования событий безопасности
        /// </summary>
        [JsonProperty("customParameters")]
        public object CustomParameters { get; set; }
    }
}
