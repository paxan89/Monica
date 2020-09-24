using System.ComponentModel.DataAnnotations.Schema;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using Monica.Core.DbModel.ModelDto.Core;

namespace Monica.Core.DbModel.ModelDto.Report
{
    public class FieldAccessDto : BaseModelDto
    {
        public FieldAccessDto()
        {
            TypeProfileForm = TypeProfileForm.Advisably;
        }

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Имя поля в БД
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Тип компонента
        /// </summary>
        public TypeField TypeField { get; set; }
        /// <summary>
        /// Тип контрола. Определяется автоматически
        /// </summary>
        public TypeControl TypeControl { get; set; }
        /// <summary>
        /// Класс выбора данных. Например для выпадающих списков 
        /// </summary>
        public string ClassData { get; set; }
        /// <summary>
        /// Порядковый номер
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Признак, что данное поле является ключем
        /// </summary>
        public bool? IsKey { get; set; }
        /// <summary>
        /// Доступ поля
        /// </summary>
        public TypeAccec TypeAccec { get; set; }
        /// <summary>
        /// Настройка профиля. Обязательно поле для заполнения или нет
        /// </summary>
        public TypeProfileForm TypeProfileForm { get; set; }
        /// <summary>
        /// Количество символов для строковых полей
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// Значение для поля
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Ссылка на форму описателя. Нужно для того чтобы определять как заполнять то или иное информационное поле
        /// </summary>
        public int FormModelId { get; set; }
        /// <summary>
        /// Таблица для колонки
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Колонка для связи между таблицами
        /// </summary>
        public string ColumnIdJoinTable { get; set; }
        /// <summary>
        /// Признак видимости на экране
        /// </summary>
        public bool? IsVisibleList { get; set; }
        /// <summary>
        /// Ширина в процентах для списка
        /// </summary>
        public int? WidthList { get; set; }
        /// <summary>
        /// Тип группы
        /// </summary>
        public TypeGroup TypeGroup { get; set; }
        /// <summary>
        /// Ссылка на родителя. То есть на группу
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// Имя группы
        /// </summary>
        public string ParentGroup { get; set; }
        /// <summary>
        /// Признак что поле виртуальное и будет дополнительно вычисляться
        /// </summary>
        public bool? IsVirtual { get; set; }
        /// <summary>
        /// Маска для форматирования
        /// </summary>
        public string Mask { get; set; }
        /// <summary>
        /// Выражение для виртуального поля. Без него не будет формироваться данные для этого поля
        /// </summary>
        public string Express { get; set; }
        /// <summary>
        /// Использовать в деталях
        /// </summary>
        public bool? IsDetail { get; set; }

        /// <summary>
        /// К какой группе относится данный блок. По умолчанию всегда будет относится к ревой группе
        /// </summary>
        public int GroupCol { get; set; } = 1;
        /// <summary>
        /// Позиция в форме для редактирования
        /// </summary>
        public int? OrderDetail { get; set; }
        /// <summary>
        /// Значения для выпадающего списка
        /// </summary>
        public string ValueListBox { get; set; }
        /// <summary>
        /// Ссылка на модель для детальной информации
        /// </summary>
        public int? FormModelDetailId { get; set; }
        /// <summary>
        /// Признак связись по имени столбца в двух таблицах
        /// </summary>
        public bool? IsOneToOne { get; set; }
        /// <summary>
        /// Сортировка элемента. Если это поле указывается до элементы сортируются по возрастанию
        /// </summary>
        public int? Sorting { get; set; }
    }
}
