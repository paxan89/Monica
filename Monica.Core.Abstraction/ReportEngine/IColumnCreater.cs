using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelDto.Report;

namespace Monica.Core.Abstraction.ReportEngine
{
    /// <summary>
    /// Класс для создания описателя для колонок
    /// </summary>
    public interface IColumnCreater
    {
        /// <summary>
        /// Получить колонки в формате json
        /// </summary>
        /// <param name="fields">Список доступных колонок</param>
        /// <returns></returns>
        Task<string> GetColumns(IEnumerable<FieldAccessDto> fields);

        /// <summary>
        /// Получить список полей для редактирования
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<string> GetProperty(IEnumerable<FieldAccessDto> fields, FormModelDto entity);
    }
}
