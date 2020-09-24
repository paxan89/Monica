using System.Collections.Generic;

namespace Monica.Core.DbModel.ModelDto.Report
{
    /// <summary>
    /// Основаная модель для сохранения данных полей режима
    /// </summary>
    public class FieldValueSaveDto
    {
        public string FieldName { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    /// Модель данных для сохранения строки поля
    /// </summary>
    public class FieldModelDto
    {
        public string Id { get; set; }
        public List<FieldValueSaveDto> FieldsValueSave { get; set; }
        public List<FormModelSave> Details { get; set; }
        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// Основаная модель для сохранения данных для режима
    /// </summary>
    public class FormModelSave
    {
        public int? FormId { get; set; }
        public List<FieldModelDto> FieldValueSaveDtos { get; set; }
    }
}
