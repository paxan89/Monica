using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.DbModel.ModelDto.Report;

namespace Monica.Core.Abstraction.ReportEngine
{
    /// <summary>
    /// Менеджер для работы с работы с конфигуратором режимов CRM
    /// </summary>
    public interface IReportManager
    {
        /// <summary>
        /// Получить список режимов, для которых у роли есть разрешения
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="isAll"></param>
        /// <returns>Список доступных режимов для пользователя</returns>
        Task<IEnumerable<FormModelDto>> GetFormsAsunc(string userName, bool isAll);

        /// <summary>
        /// Получить список доступа на каждый элемент
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="formId">Режим для которого нужно выбрать доступ</param>
        /// <param name="isAll">Выбрать все записи (с удаленными)</param>
        /// <param name="whereFields"></param>
        /// <returns></returns>
        Task<IEnumerable<FieldAccessDto>> GetFieldsFormAsync(string userName, int formId, bool isAll, Func<IQueryable<Field>, IQueryable<Field>> whereFields = null);

        /// <summary>
        /// Получить список доступа на каждый элемент
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="formId">Режим для которого нужно выбрать доступ</param>
        /// <param name="isAll">Выбрать все записи (с удаленными)</param>
        /// <returns></returns>
        Task<IEnumerable<ButtonAccessDto>> GetButtonsAsync(string userName, int formId, bool isAll);

        /// <summary>
        /// Добавление новой формы
        /// </summary>
        /// <param name="modelArg">Модель данных для добавления</param>
        /// <returns>Результат выполнения</returns>
        Task<ResultCrmDb> AddOrEditFormReportAsync(FormModelDto modelArg);

        /// <summary>
        /// Удаление режима
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        Task<ResultCrmDb> RemoveFormReportAsync(int formId);

        /// <summary>
        /// Добавление или редактирование полей
        /// </summary>
        /// <param name="fieldDto">Модель для сохранения поля</param>
        /// <returns>Результат выполнения</returns>
        Task<ResultCrmDb> AddOrEditFieldAsync(FieldAccessDto fieldDto);

        /// <summary>
        /// Удаление поля
        /// </summary>
        /// <param name="modelId">Уникальный ключ поля</param>
        /// <returns>Результат выполнения</returns>
        Task<ResultCrmDb> RemoveFieldAsync(int modelId);

        /// <summary>
        /// Добавление или редактирование полей
        /// </summary>
        /// <param name="fieldDto">Модель для сохранения поля</param>
        /// <returns>Результат выполнения</returns>
        Task<ResultCrmDb> AddOrEditButtonAsync(ButtonAccessDto fieldDto);

        /// <summary>
        /// Удаление ryjgrb
        /// </summary>
        /// <param name="modelId">Уникальный ключ поля</param>
        /// <returns>Результат выполнения</returns>
        Task<ResultCrmDb> RemoveButtonAsync(int modelId);


        /// <summary>
        /// Обновление или добавление нового типа формы
        /// </summary>
        /// <param name="typeForm"></param>
        /// <returns></returns>
        Task<ResultCrmDb> AddOrEditTypeForm(TypeForm typeForm);


        /// <summary>
        /// Удаление типа режима (возможно только если режим нигде не используется)
        /// </summary>
        /// <param name="modelId">Уникальный ключ поля</param>
        /// <returns>Результат выполнения</returns>
        Task<ResultCrmDb> RemoveTypeFormAsync(int modelId);

        /// <summary>
        /// Получить модель модель для основного ввода информации
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FormModelDto> GetFormModel(int id);

        /// <summary>
        /// Получить модель поля
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FieldAccessDto> GetFieldModel(int id);

        /// <summary>
        /// Получить модель кнопки для редактирования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ButtonAccessDto> GetButtonModel(int id);

        /// <summary>
        /// Получить список доступа на каждый элемент
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="formId">Режим для которого нужно выбрать доступ</param>
        /// <param name="isAll">Выбрать все записи (с удаленными)</param>
        /// <param name="whereFields"></param>
        /// <returns></returns>
        Task<IEnumerable<FieldAccessDto>> GetFieldsFormWithProfileAsync(string userName, int formId, bool isAll, Func<IQueryable<Field>, IQueryable<Field>> whereFields = null);

        /// <summary>
        /// Получить список типов редимов
        /// </summary>
        /// <returns>Список режимов</returns>
        Task<IEnumerable<TypeForm>> GetTypeForms();

        /// <summary>
        /// Получение дерева моделей
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="isAll">Показать все режимы и даже удаленные</param>
        /// <returns></returns>
        Task<IEnumerable<SolutionModel>> GetSolutionModel(string userName, bool isAll);

        /// <summary>
        /// Получить список полей для группы
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        Task<IEnumerable<FieldAccessDto>> GetFieldByFormsOnlyGroup(string userName, int formId);


        /// <summary>
        /// Получить список объектов валидации для сущности
        /// </summary>
        /// <param name="formId">Системный номер модели</param>
        /// <returns></returns>
        Task<List<ValidationRuleEntityDto>> GetValidationRuleEntity(int formId);

        /// <summary>
        /// Добавить новое правило для сохранения модели
        /// </summary>
        /// <param name="model">Модель для добавления</param>
        /// <returns></returns>
        Task<ResultCrmDb> AddValidationRule(ValidationRuleEntityDto model);

        /// <summary>
        /// Удалить правило для сохранения модели
        /// </summary>
        /// <param name="validateId"></param>
        /// <returns></returns>
        Task<ResultCrmDb> RemoveValidationRule(int validateId);
    }
}
