using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm.Core;

namespace Monica.Core.DbModel.ModelCrm.EngineReport
{
    /// <summary>
    /// Модель для правил валидации объекта БД
    /// </summary>
    public class ValidationRuleEntity : BaseModel
    {
        /// <summary>
        /// Имя - подсказка для объекта валидации
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Выполняемая команда или SQL объект
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Тип валидации. Или Sql команда или компонент
        /// </summary>
        public TypeValidation TypeValidation { get; set; }
        /// <summary>
        /// Тип возвращаемых данных (или результат bool, или объект данных)
        /// </summary>
        public TypeReturnValidation TypeReturnValidation { get; set; }
        /// <summary>
        /// Подсказки, которые передаются на клиент с сервера
        /// </summary>
        public string ToolTip { get; set; }
        /// <summary>
        /// Ссылка на сущность
        /// </summary>
        public int FormModelId { get; set; }
    }
}
