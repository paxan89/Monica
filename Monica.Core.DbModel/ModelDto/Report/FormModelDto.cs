using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelDto.Core;

namespace Monica.Core.DbModel.ModelDto.Report
{
    public class FormModelDto : BaseModelDto
    {
        /// <summary>
        /// Заголовок режима
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// Имя таблицы для которой будет формироваться список
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Порядковый номер
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Ссылка на тип формы
        /// </summary>
        public int TypeFormId { get; set; }
        /// <summary>
        /// Тип модификации режима
        /// </summary>
        public TypeField TypeEditor { get; set; }
        /// <summary>
        /// Разположение компонентов и вся необходимая информация для правильного расположения компонентов на форме
        /// </summary>
        public string JsonMarkingStyle { get; set; }
        /// <summary>
        /// Имя параметризированного класса, для которого будет формироваться данные
        /// </summary>
        public string NameClassDataEngine { get; set; }

        public string VueComponent { get; set; }
        /// <summary>
        /// Количество колонок в форме для редактирования
        /// </summary>
        public int? ColCount { get; set; } = 1;
        /// <summary>
        /// Ширина для формы редактирования
        /// </summary>
        public int? WidthDetail { get; set; }
        /// <summary>
        /// Высота для формы редактирования
        /// </summary>
        public int? HeightDetail { get; set; }
        public bool? IsNotVisible { get; set; }
    }
}
