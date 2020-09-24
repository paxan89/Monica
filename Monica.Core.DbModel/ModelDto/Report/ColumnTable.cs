namespace Monica.Core.DbModel.ModelDto.Report
{
    /// <summary>
    /// Колонки для режима
    /// </summary>
    public class ColumnTable
    {
        /// <summary>
        /// Имя колонки
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// Тип данных
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// Длина строкового значения
        /// </summary>
        public int? Maxlength { get; set; }
        /// <summary>
        /// Указание что колонка относится к первичному ключу
        /// </summary>
        public string ColumnKey { get; set; }
        /// <summary>
        /// Имя таблицы
        /// </summary>
        public string TableName { get; set; }

        public bool IsOneToOne { get; set; }
    }
}
