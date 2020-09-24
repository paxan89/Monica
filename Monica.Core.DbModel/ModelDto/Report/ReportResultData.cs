using System.Collections.Generic;

namespace Monica.Core.DbModel.ModelDto.Report
{
    /// <summary>
    /// Модель результата выбора данных
    /// </summary>
    public class ReportResultData
    {
        /// <summary>
        /// Список полей
        /// </summary>
        public IEnumerable<FieldAccessDto> FieldAccess { get; set; }
        /// <summary>
        /// Список доступных кнопок
        /// </summary>
        public string Buttons { get; set; }
        /// <summary>
        /// Данные которые находятся на форме
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// Колонки в формате JSON
        /// </summary>
        public string Columns { get; set; }
        /// <summary>
        /// Поля для редактирования формы
        /// </summary>
        public string FormProperty { get; set; }
        /// <summary>
        /// Описатель полученного режима
        /// </summary>
        public FormModelDto FormModel { get; set; }
        /// <summary>
        /// Имя ключа в модели данных
        /// </summary>
        public string KeyField { get; set; }
    }

    
}
