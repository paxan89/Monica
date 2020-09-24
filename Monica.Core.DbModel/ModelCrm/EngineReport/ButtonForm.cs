using System.ComponentModel.DataAnnotations.Schema;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm.Core;

namespace Monica.Core.DbModel.ModelCrm.EngineReport
{
    /// <summary>
    /// Кнопки на форме редактирования или на компоненте таблицы
    /// </summary>
    public class ButtonForm : BaseModel
    {
        /// <summary>
        /// Название отображаемое на экране
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Подсказка для кнопки
        /// </summary>
        public string ToolTip { get; set; }
        /// <summary>
        /// Высота кнопки
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Ширина кнопки
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Системное имя кнопки
        /// </summary>
        public string SysName { get; set; }
        /// <summary>
        /// Позиция
        /// </summary>
        public int Order { get; set; }
        public int? FormId { get; set; }
        [ForeignKey("FormId")]
        public FormModel FormModel { get; set; }
        /// <summary>
        /// Имя картинки
        /// </summary>
        public string IconName { get; set; }
        /// <summary>
        /// Признак, что кнопка применятется только для формы редактирования
        /// </summary>
        public bool IsDetail { get; set; }
        /// <summary>
        /// Тип кнопки
        /// </summary>
        public TypeBtn TypeBtn { get; set; }
        /// <summary>
        /// Тип кнопки
        /// </summary>
        public StylingMode StylingMode { get; set; }
        /// <summary>
        /// Тип кнопки
        /// </summary>
        public Location Location { get; set; }
        /// <summary>
        /// Контейнер для валидации
        /// </summary>
        public string ValidationGroup { get; set; }
    }
}
