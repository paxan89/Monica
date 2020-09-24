using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.Core;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.DbModel.ModelDto.Report;

namespace Monica.Core.Service.ReportEngine
{
    /// <summary>
    /// Менеджер для работы с работы с конфигуратором режимов CRM
    /// </summary>
    public class ReportManager : IReportManager
    {
        private readonly ReportDbContext _reportDbContext;
        private readonly IAccessManager _accessManager;

        public ReportManager(ReportDbContext reportDbContext, IAccessManager accessManager)
        {
            _reportDbContext = reportDbContext;
            _accessManager = accessManager;
        }
        
        /// <summary>
        /// Получить список режимов, для которых у роли есть разрешения
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="isAll">Выбрать все записи (с удаленными)</param>
        /// <returns>Список доступных режимов для пользователя</returns>
        public async Task<IEnumerable<FormModelDto>> GetFormsAsunc(string userName, bool isAll)
        {
            var models = isAll
                ? await _reportDbContext.FormModel.ToListAsync()
                : await _reportDbContext.FormModel.Where(f => !(f.IsDeleted ?? false)).ToListAsync();
            var query = models.GroupJoin(
                await _accessManager.GetAccessFormsAsync(userName)
                , model => model.Id,
                accessForm => accessForm.FormModelId,
                (formModel, accessForms) => new { Form = formModel, Access = accessForms }).SelectMany(
                arg => arg.Access.DefaultIfEmpty(),
                (form, access) => new
                {
                    form.Form,
                    IsVisible = access == null || new[] { TypeAccec.Full, TypeAccec.ReadOnly }.Contains(access.TypeAccec)
                }).Where(f => f.IsVisible);
            return query.Select(s => s.Form.Map<FormModelDto>()).OrderBy(o => o.Order).ToList();
        }

        /// <summary>
        /// Получение дерева моделей
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="isAll">Показать все режимы и даже удаленные</param>
        /// <returns></returns>
        public async Task<IEnumerable<SolutionModel>> GetSolutionModel(string userName, bool isAll)
        {
            var formModel = await GetFormsAsunc(userName, isAll);

            var typeForms = await _reportDbContext.TypeForm.ToListAsync();
            var models = new List<SolutionModel>();
            int i = 1;
            int parentId = 0;

            if (!isAll)
            {
                models.Add(new SolutionModel
                {
                    Text = "Главная страница",
                    Expanded = true,
                    Id = 0,
                    VueComponent = "MainView"
                });
                formModel = formModel.Where(f => !(f.IsNotVisible ?? false));
            }
            RecurceAddModels(formModel.ToList(),typeForms, models, null, ref i, ref parentId);
            return models;
        }

        private void RecurceAddModels(List<FormModelDto> forms, List<TypeForm> typeForms, List<SolutionModel> models, TypeForm parent, ref int i, ref int parentId)
        {
            foreach (var typeForm in typeForms.Where(f => f.ParentId == parent?.Id))
            {
                var model = new SolutionModel();
                model.Id = i;
                model.IdModel = -1;
                model.Expanded = true;
                model.Text = typeForm.DisplayName;
                model.ParentId = typeForm.ParentId == null ? 0 : parentId;
                models.Add(model);
                parentId = i;
                i++;
                foreach (var formModelDto in forms.Where(f => f.TypeFormId == typeForm.Id))
                {
                    var solFormDto = new SolutionModel();
                    solFormDto.Id = i;
                    solFormDto.Text = formModelDto.Caption;
                    solFormDto.ParentId = model.Id;
                    solFormDto.Expanded = true;
                    solFormDto.IdModel = formModelDto.Id;
                    solFormDto.TableName = formModelDto.TableName;
                    solFormDto.VueComponent = string.IsNullOrEmpty(formModelDto.VueComponent)
                        ? "DesignerDictionary"
                        : formModelDto.VueComponent;
                    models.Add(solFormDto);
                    i++;
                }
                RecurceAddModels(forms, typeForms, models, typeForm, ref i, ref parentId);
            }
        }

        /// <summary>
        /// Получить список доступа на каждый элемент
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="formId">Режим для которого нужно выбрать доступ</param>
        /// <param name="isAll">Выбрать все записи (с удаленными)</param>
        /// <param name="whereFields"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FieldAccessDto>> GetFieldsFormAsync(string userName, int formId, bool isAll, Func<IQueryable<Field>, IQueryable<Field>> whereFields = null)
        {
            var modelsWhere = isAll
                ? _reportDbContext.Field.Where(f => f.FormModelId == formId)
                : _reportDbContext.Field.Where(f => f.FormModelId == formId && !(f.IsDeleted ?? false));

            if (whereFields != null)
                modelsWhere = whereFields(modelsWhere);

            var models = await modelsWhere.ToListAsync();
            var accessFormDefault =
                await _reportDbContext.AccessForm.FirstOrDefaultAsync(
                    f => f.FormModelId == formId && f.FieldId == null && f.ButtonFormId == null);
            var defAccess = accessFormDefault == null ? 3 : (int)accessFormDefault.TypeAccec;
            var query =  models.GroupJoin(
                await _accessManager.GetAccessFildsByFormAsync(userName, formId)
                , model => model.Id,
                accessForm => accessForm.FieldId,
                (formModel, accessForms) => new { Form = formModel, Access = accessForms }).SelectMany(
                arg => arg.Access.DefaultIfEmpty(),
                (form, access) => new
                {
                    form.Form,
                    TypeAccec = access == null ? defAccess :
                        (int)form.Form.DefaultTypeAccec > defAccess ? defAccess : (int)access.TypeAccec
                }).Where(f => f.TypeAccec > 1);
            return query.Select(s =>
                    {
                        var item = Mapper.Map<FieldAccessDto>(s.Form);
                        item.ParentGroup = $"{s.Form.Parent?.DisplayName} ({s.Form.Parent?.Name}";
                        item.TypeAccec = (TypeAccec)s.TypeAccec;
                        return item;
                    }).OrderBy(o => o.Order);
        }

        /// <summary>
        /// Получить список доступа на каждый элемент
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="formId">Режим для которого нужно выбрать доступ</param>
        /// <param name="isAll">Выбрать все записи (с удаленными)</param>
        /// <returns></returns>
        public async Task<IEnumerable<FieldAccessDto>> GetFieldsFormWithProfileAsync(string userName, int formId, bool isAll, Func<IQueryable<Field>, IQueryable<Field>> whereFields = null)
        {
            var modelsFileds = (await GetFieldsFormAsync(userName, formId, isAll, whereFields)).ToList();
            var query = modelsFileds.
                Join(await _accessManager.GetPropfileForm(userName, formId), 
                    dto => dto.Id,
                    form => form.Id, (dto, form) => new { dto, form.TypeProfileForm });

            foreach (var model in query)
            {
                model.dto.TypeProfileForm = model.TypeProfileForm;
            }

            return modelsFileds;
        }
        /// <summary>
        /// Получить список полей для группы
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FieldAccessDto>> GetFieldByFormsOnlyGroup(string userName, int formId)
        {
            var result = await GetFieldsFormAsync(userName, formId, true);
            return result.Where(f => f.TypeGroup != TypeGroup.None).Select(s =>
            {
                s.DisplayName = string.IsNullOrWhiteSpace(s.DisplayName) ? s.Name : s.DisplayName;
                return s;
            });
        }

        /// <summary>
        /// Получить список объектов валидации для сущности
        /// </summary>
        /// <param name="formId">Системный номер модели</param>
        /// <returns></returns>
        public async Task<List<ValidationRuleEntityDto>> GetValidationRuleEntity(int formId)
        {
            return (await _reportDbContext.ValidationRuleEntity.Where(f => f.FormModelId == formId).ToListAsync())
                .Select(s => s.Map<ValidationRuleEntityDto>()).ToList();
        }

        /// <summary>
        /// Добавить новое правило для сохранения модели
        /// </summary>
        /// <param name="model">Модель для добавления</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> AddValidationRule(ValidationRuleEntityDto model)
        {
            return await AddOrEditModel(model, _reportDbContext.ValidationRuleEntity);
        }

        /// <summary>
        /// Удалить правило для сохранения модели
        /// </summary>
        /// <param name="validateId"></param>
        /// <returns></returns>
        public async Task<ResultCrmDb> RemoveValidationRule(int validateId)
        {
            return await RemoveModel(validateId, _reportDbContext.ValidationRuleEntity);
        }

        /// <summary>
        /// Получить список доступа на каждый элемент
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="formId">Режим для которого нужно выбрать доступ</param>
        /// <param name="isAll">Выбрать все записи (с удаленными)</param>
        /// <returns></returns>
        public async Task<IEnumerable<ButtonAccessDto>> GetButtonsAsync(string userName, int formId, bool isAll)
        {
            var models = isAll
                ? await _reportDbContext.ButtonForm.Where(f => f.FormId == formId).ToListAsync()
                : await _reportDbContext.ButtonForm.Where(f => f.FormId == formId && !(f.IsDeleted ?? false)).ToListAsync();
            var query =  models.GroupJoin(
                await _accessManager.GetAccessButtonByFormAsync(userName, formId)
                , model => model.Id,
                accessForm => accessForm.ButtonFormId,
                (formModel, accessForms) => new { Button = formModel, Access = accessForms }).SelectMany(
                arg => arg.Access.DefaultIfEmpty(),
                (form, access) => new
                {
                    form.Button,
                    IsVisible = access == null || (int)access.TypeAccec > 2
                }).Where(f => f.IsVisible);
            return query.Select(s =>
            {
                var item = Mapper.Map<ButtonAccessDto>(s.Button);
                return item;
            }).OrderBy(o => o.Order);
        }

        /// <summary>
        /// Добавление новой формы
        /// </summary>
        /// <param name="modelArg">Модель данных для добавления</param>
        /// <returns>Результат выполнения</returns>
        public async Task<ResultCrmDb> AddOrEditFormReportAsync(FormModelDto modelArg)
        {
            if (modelArg.Id == 0 && string.IsNullOrWhiteSpace(modelArg.NameClassDataEngine))
            {
                modelArg.NameClassDataEngine = nameof(ReportEngineDefaultData);
            }
            return await AddOrEditModel(modelArg, _reportDbContext.FormModel);
        }
        /// <summary>
        /// Удаление режима
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        public async Task<ResultCrmDb> RemoveFormReportAsync(int formId)
        {
            return await FullRemoveModel(formId, _reportDbContext.FormModel);
        }

        /// <summary>
        /// Добавление или редактирование полей
        /// </summary>
        /// <param name="fieldDto">Модель для сохранения поля</param>
        /// <returns>Результат выполнения</returns>
        public async Task<ResultCrmDb> AddOrEditFieldAsync(FieldAccessDto fieldDto)
        {
            return await AddOrEditModel(fieldDto, _reportDbContext.Field);
        }

        /// <summary>
        /// Удаление поля
        /// </summary>
        /// <param name="modelId">Уникальный ключ поля</param>
        /// <returns>Результат выполнения</returns>
        public async Task<ResultCrmDb> RemoveFieldAsync(int modelId)
        {
            return await FullRemoveModel(modelId, _reportDbContext.Field);
        }


        /// <summary>
        /// Получить модель модель для основного ввода информации
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FormModelDto> GetFormModel(int id)
        {
            var model = await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == id);
            return model.Map<FormModelDto>();
        }
        /// <summary>
        /// Получить модель поля
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FieldAccessDto> GetFieldModel(int id)
        {
            var model = await _reportDbContext.Field.FirstOrDefaultAsync(f => f.Id == id);
            return model.Map<FieldAccessDto>();
        }

        /// <summary>
        /// Получить модель кнопки для редактирования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ButtonAccessDto> GetButtonModel(int id)
        {
            var model = await _reportDbContext.ButtonForm.FirstOrDefaultAsync(f => f.Id == id);
            return model.Map<ButtonAccessDto>();
        }

        /// <summary>
        /// Добавление или редактирование кнопки
        /// </summary>
        /// <param name="fieldDto">Модель для сохранения поля</param>
        /// <returns>Результат выполнения</returns>
        public async Task<ResultCrmDb> AddOrEditButtonAsync(ButtonAccessDto fieldDto)
        {
            return await AddOrEditModel(fieldDto, _reportDbContext.ButtonForm);
        }

        /// <summary>
        /// Удаление кнопки
        /// </summary>
        /// <param name="modelId">Уникальный ключ поля</param>
        /// <returns>Результат выполнения</returns>
        public async Task<ResultCrmDb> RemoveButtonAsync(int modelId)
        {
            return await RemoveModel(modelId, _reportDbContext.ButtonForm);
        }

        /// <summary>
        /// Получить список типов редимов
        /// </summary>
        /// <returns>Список режимов</returns>
        public async Task<IEnumerable<TypeForm>> GetTypeForms()
        {
            return await _reportDbContext.TypeForm.ToArrayAsync();
        }

        /// <summary>
        /// Обновление или добавление нового типа формы
        /// </summary>
        /// <param name="typeForm"></param>
        /// <returns></returns>
        public async Task<ResultCrmDb> AddOrEditTypeForm(TypeForm typeForm)
        {
           return await AddOrEditModel(typeForm, _reportDbContext.TypeForm);
        }
        
        /// <summary>
        /// Удаление типа режима (возможно только если режим нигде не используется)
        /// </summary>
        /// <param name="modelId">Уникальный ключ поля</param>
        /// <returns>Результат выполнения</returns>
        public async Task<ResultCrmDb> RemoveTypeFormAsync(int modelId)
        {
            return await FullRemoveModel(modelId, _reportDbContext.TypeForm);
        }

        private async Task<ResultCrmDb> AddOrEditModel<TModel,TModelEdit>(TModelEdit modelDto, DbSet<TModel> models)
            where TModel : BaseModel
            where TModelEdit : IBaseModel
        {
            var result = new ResultCrmDb();
            try
            {
                var model = await models.FirstOrDefaultAsync(f => f.Id == modelDto.Id);
                model = modelDto.Map(model);
                if (model.Id == 0)
                    _reportDbContext.Add(model);
                else _reportDbContext.Update(model);
                await _reportDbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.AddError(ErrorCode.NoCode.ToString(), e.Message);
            }

            return result;
        }

        private async Task<ResultCrmDb> FullRemoveModel<TModel>(int modelId, DbSet<TModel> models)
            where TModel : BaseModel
        {
            var result = new ResultCrmDb();
            try
            {
                var model = await models.FirstOrDefaultAsync(f => f.Id == modelId);
                if (model == null)
                    return result;
                _reportDbContext.Remove(model);
                await _reportDbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.AddError(ErrorCode.NoCode.ToString(), e.Message);
            }

            return result;
        }

        private async Task<ResultCrmDb> RemoveModel<TModel>(int modelId, DbSet<TModel> models)
            where TModel : BaseModel
        {
            var result = new ResultCrmDb();
            try
            {
                var model = await models.FirstOrDefaultAsync(f => f.Id == modelId);
                if (model == null)
                    return result;
                model.IsDeleted = true;
                _reportDbContext.Update(model);
                await _reportDbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.AddError(ErrorCode.NoCode.ToString(), e.Message);
            }

            return result;
        }

    }
}
