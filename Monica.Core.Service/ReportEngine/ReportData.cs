using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.DbModel.ModelDto.Core;
using Monica.Core.DbModel.ModelDto.Report;
using Monica.Core.Exceptions;
using Monica.Core.Service.Extension;
using Monica.Core.Utils;
using Newtonsoft.Json.Linq;

namespace Monica.Core.Service.ReportEngine
{
    /// <summary>
    /// Получение данных универсальным путем
    /// </summary>
    public class ReportData : IReportData
    {
        private readonly ReportDbContext _reportDbContext;
        private readonly IReportManager _reportManager;
        private readonly IAccessManager _accessManager;
        private readonly IColumnCreater _columnCreater;
        private readonly IButtonCreater _buttonCreater;

        public ReportData(ReportDbContext reportDbContext, IReportManager reportManager, IAccessManager accessManager, IColumnCreater columnCreater, IButtonCreater buttonCreater)
        {
            _reportDbContext = reportDbContext;
            _reportManager = reportManager;
            _accessManager = accessManager;
            _columnCreater = columnCreater;
            _buttonCreater = buttonCreater;
        }

        /// <summary>
        /// Получить данные для списка
        /// </summary>
        /// <param name="parametr"></param>
        /// <returns></returns>
        public async Task<ReportResultData> GetDataList(BaseModelReportParam parametr)
        {
            var resultData = new ReportResultData();
            var accessForm = await _accessManager.GetAccessFormAsync(parametr.UserName, parametr.FormId);
            if (accessForm != null && (int)accessForm.TypeAccec < 2)
                throw new UserMessageException("Вы запросили данные режима, на который у вас не открыты права");
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == parametr.FormId)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            resultData.FormModel = formModelDto;
            resultData.FieldAccess = await _reportManager.GetFieldsFormWithProfileAsync(parametr.UserName, parametr.FormId, false, 
                fields => fields.Where(f => (f.IsVisibleList ?? false) || string.IsNullOrWhiteSpace(f.Express)));
            
            var report = AutoFac.ResolveNamed<IReportEngineData>(string.IsNullOrWhiteSpace(formModelDto.NameClassDataEngine) ? nameof(ReportEngineDefaultData) : formModelDto.NameClassDataEngine);

            var resultDataFieldAccess = resultData.FieldAccess as FieldAccessDto[] ?? resultData.FieldAccess.ToArray();

            resultData.Data = GetDataAccess(report == null ? null : await report.GetDataList(parametr), resultDataFieldAccess);
            resultData.Columns = await _columnCreater.GetColumns(resultDataFieldAccess);
            resultData.KeyField = resultDataFieldAccess.FirstOrDefault(f => f.IsKey ?? false)?.Name;
            resultData.FormProperty = await _columnCreater.GetProperty(resultDataFieldAccess, formModelDto);
            return resultData;
        }


        /// <summary>
        /// Получить модель данных для редактирования детальной информации
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SolutionModel>> GetDetailInfoModels(int formModel, string userName)
        {
            var accessForm = await _accessManager.GetAccessFormAsync(userName, formModel);
            if (accessForm != null && (int)accessForm.TypeAccec < 2)
                throw new UserMessageException("Вы запросили данные режима, на который у вас не открыты права");
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModel)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IReportEngineData>(string.IsNullOrWhiteSpace(formModelDto.NameClassDataEngine) ? nameof(ReportEngineDefaultData) : formModelDto.NameClassDataEngine);

            return await report.GetDetailInfoModels(formModelDto);
        }

        /// <summary>
        /// Получить список кнопок для режима
        /// </summary>
        /// <param name="formId">Id режима</param>
        /// <param name="userName">Имя пользователя</param>
        /// <returns></returns>
        public async Task<JArray> GetButtons(int formId, string userName)
        {
            var buttons = (await _reportManager.GetButtonsAsync(userName, formId, false)).Where(f => !f.IsDetail);
            return await _buttonCreater.GetButtonJson(buttons);
        }


        /// <summary>
        /// Получить список кнопок для режима
        /// </summary>
        /// <param name="formId">Id режима</param>
        /// <param name="userName">Имя пользователя</param>
        /// <returns></returns>
        public async Task<JArray> GetButtonsDetail(int formId, string userName)
        {
            var buttons = (await _reportManager.GetButtonsAsync(userName, formId, false)).Where(f => f.IsDetail);
            return await _buttonCreater.GetButtonJson(buttons);
        }

        public async Task<ResultCrmDb> SaveModel(dynamic saveModel, int typeEditor, int formModel, string userName)
        {
            var accessForm = await _accessManager.GetAccessFormAsync(userName, formModel);
            if (accessForm != null && (int)accessForm.TypeAccec < 2)
                throw new UserMessageException("Вы запросили данные режима, на который у вас не открыты права");
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModel)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IReportEngineData>(string.IsNullOrWhiteSpace(formModelDto.NameClassDataEngine) ? nameof(ReportEngineDefaultData) : formModelDto.NameClassDataEngine);
            return await report.SaveModels(formModelDto,userName, saveModel);
        }


        /// <summary>
        /// Проверка сохранеяемой модели по предоставленным правилам
        /// </summary>
        /// <param name="saveModel">Модель для сохранения</param>
        /// <param name="formModelId">Системный номер модели данных</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> ValidateModel(dynamic saveModel, int formModelId)
        {
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModelId)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IReportEngineData>(string.IsNullOrWhiteSpace(formModelDto.NameClassDataEngine) ? nameof(ReportEngineDefaultData) : formModelDto.NameClassDataEngine);
            return await report.ValidateModel(saveModel, formModelDto);
        }

        /// <summary>
        /// Удаление моделей данных
        /// </summary>
        /// <param name="formModel">Сущность для которой нужно удалить данные</param>
        /// <param name="userName">Пользователь, который удаляет данные</param>
        /// <param name="key">Список ключей сущности, которые нужно удалить</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> RemoveEntity(int formModel, string userName, string[] key)
        {
            var accessForm = await _accessManager.GetAccessFormAsync(userName, formModel);
            if (accessForm != null && (int)accessForm.TypeAccec < 2)
                throw new UserMessageException("Вы запросили данные режима, на который у вас не открыты права");
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModel)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IReportEngineData>(string.IsNullOrWhiteSpace(formModelDto.NameClassDataEngine) ? nameof(ReportEngineDefaultData) : formModelDto.NameClassDataEngine);
            return await report.RemoveEntity(formModelDto, userName, key);

        }


        /// <summary>
        /// Получить данные для списка
        /// </summary>
        /// <param name="parametr"></param>
        /// <returns></returns>
        public async Task<ReportResultData> GetEditModel(BaseModelReportParam parametr)
        {
            var resultData = new ReportResultData();
            var accessForm = await _accessManager.GetAccessFormAsync(parametr.UserName, parametr.FormId);
            if (accessForm != null && (int)accessForm.TypeAccec < 2)
                throw new UserMessageException("Вы запросили данные режима, на который у вас не открыты права");
            var formModelDto =
                Mapper.Map<FormModelDto>((
                    await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == parametr.FormId)));
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");

            resultData.FormModel = formModelDto;
            resultData.FieldAccess = await _reportManager.GetFieldsFormWithProfileAsync(parametr.UserName, parametr.FormId, false, fields => fields.Where(f => f.IsDetail ?? false));
            // resultData.Buttons = await _reportManager.GetButtonsAsync(parametr.UserName, parametr.FormId, false);
            var report = AutoFac.ResolveNamed<IReportEngineData>(formModelDto.NameClassDataEngine);
            var resultDataFieldAccess = resultData.FieldAccess as FieldAccessDto[] ?? resultData.FieldAccess.ToArray();
            resultData.Data = report == null ? "" : GetModelAccess(await report.GetDataEditModel(parametr), resultDataFieldAccess);
            return resultData;
        }



        private string GetDataAccess(IEnumerable<IDictionary<string, object>> data, IEnumerable<FieldAccessDto> fields)
        {
            var jArray = new JArray();
            if (data == null)
                return jArray.ToString();
            foreach (var model in data)
            {
                var jObject = new JObject();
                foreach (var fieldAccessDto in fields)
                {
                    var proper = model.FirstOrDefault(f => f.Key.ToLower() == ColumnHelper.GetFieldName(fieldAccessDto.Name)?.ToLower());
                    if (string.IsNullOrWhiteSpace(proper.Key))
                        continue;
                    jObject.Add(proper.Key, new JValue(proper.Value));
                }
                jArray.Add(jObject);
            }

            return jArray.ToString();
        }

        private string GetModelAccess(IDictionary<string, object> model, IEnumerable<FieldAccessDto> fields)
        {
            var jObject = new JObject();
            foreach (var fieldAccessDto in fields)
            {
                var proper = model.FirstOrDefault(f => f.Key.ToLower() == ColumnHelper.GetFieldName(fieldAccessDto.Name)?.ToLower());
                if (string.IsNullOrWhiteSpace(proper.Key))
                    continue;
                jObject.Add(proper.Key, new JValue(proper.Value));
            }

            return jObject.ToString();
        }
    }
}
