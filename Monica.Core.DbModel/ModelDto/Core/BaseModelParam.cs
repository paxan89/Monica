namespace Monica.Core.DbModel.ModelDto.Core
{
    /// <summary>
    /// Базовый класс параметров
    /// </summary>
    public class BaseModelReportParam
    {
        public int FormId { get; set; }
        public int FormIdByDetail { get; set; }
        public string FieldWhere { get; set; }
        public string ModelId { get; set; }
        public string UserName { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
    }
}
