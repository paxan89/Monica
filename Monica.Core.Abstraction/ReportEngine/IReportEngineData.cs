using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.DbModel.ModelDto.Core;
using Monica.Core.DbModel.ModelDto.Report;

namespace Monica.Core.Abstraction.ReportEngine
{
    /// <summary>
    /// Базовый класс для получения модели данных для запрашиваемых режимов
    /// </summary>
    public interface IReportEngineData
    {
        /// <summary>
        /// Получить данные для конкретного режима
        /// </summary>
        /// <param name="p">Параметры для формирования данных</param>
        /// <returns>Список данных</returns>
        Task<IEnumerable<IDictionary<string, object>>> GetDataList(BaseModelReportParam p);

        /// <summary>
        /// Получить данные для формы редктирования записи
        /// </summary>
        /// <param name="p"></param>
        /// <returns>Модель для редактирования</returns>
        Task<IDictionary<string, object>> GetDataEditModel(BaseModelReportParam p);

        /// <summary>
        /// Сохранение измененных данных с фронта
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="formModelSave">Модель для сохранения данных</param>
        /// <param name="formModel"></param>
        /// <returns></returns>
        Task<ResultCrmDb> SaveModels(FormModelDto formModel, string userName, dynamic formModelSave);


        /// <summary>
        /// Удаление моделей данных
        /// </summary>
        /// <param name="formModel">Сущность для которой нужно удалить данные</param>
        /// <param name="userName">Пользователь, который удаляет данные</param>
        /// <param name="key">Список ключей сущности, которые нужно удалить</param>
        /// <returns></returns>
        Task<ResultCrmDb> RemoveEntity(FormModelDto formModel, string userName, string[] key);

        /// <summary>
        /// Проверка сохранеяемой модели по предоставленным правилам
        /// </summary>
        /// <param name="saveModel">Модель для сохранения</param>
        /// <param name="formModelId">Системный номер модели данных</param>
        /// <returns></returns>
        Task<ResultCrmDb> ValidateModel(dynamic saveModel, FormModelDto formModelId);

        /// <summary>
        /// Получить модель данных для редактирования детальной информации
        /// </summary>
        /// <param name="formModel">Модель</param>
        /// <returns></returns>
        Task<IEnumerable<SolutionModel>> GetDetailInfoModels(FormModelDto formModel);
    }
}
