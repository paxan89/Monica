using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelDto.Core;

namespace Monica.Core.DbModel.ModelDto.Report
{
    public class ButtonAccessDto : BaseModelDto
    {
        private string _icon;

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

        public string SysName { get; set; }
        /// <summary>
        /// Ссылка на форму описателя. Нужно для того чтобы определять как заполнять то или иное информационное поле
        /// </summary>
        public int FormModelId { get; set; }
        /// <summary>
        /// Позиция
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Имя картинки
        /// </summary>
        public string IconName {
            get => _icon ?? SysName?.ToLower();
            set => _icon = value;
        }
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
