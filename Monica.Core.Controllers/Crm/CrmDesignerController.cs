using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.DbModel.ModelDto.Report;
using Monica.Core.Service.ReportEngine;
using Monica.Core.Utils;

namespace Monica.Core.Controllers.Crm
{

    /// <summary>
    /// Основной контроллер для авторизации пользователей в системе
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "FunctionalAdministrator")]
    public class CrmDesignerController : BaseController
    {
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"CrmDesignerController";

        private readonly IGenerateField _generateField;
        private IReportManager _reportManager;

        /// <summary>
        /// Конструктор
        /// </summary>
        public CrmDesignerController(IReportManager reportManager) : base(ModuleName)
        {
            _reportManager = reportManager;
            _generateField = AutoFac.ResolveNamed<IGenerateField>(nameof(GenerateFieldMySql));
        }

        /// <summary>
        /// Получить список форм для редактирования
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetFormsAsunc()
        {
            var result = await _reportManager.GetFormsAsunc(GetUserName(), true);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить список полей формы
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetFieldByForms(int idForm)
        {
            return Tools.CreateResult(true, "", await _reportManager.GetFieldsFormAsync(GetUserName(),idForm, true));
        }

        /// <summary>
        /// Получить список полей формы
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetFieldByFormsOnlyGroup(int idForm)
        {
            return Tools.CreateResult(true, "", await _reportManager.GetFieldByFormsOnlyGroup(GetUserName(), idForm));
        }

        /// <summary>
        /// Получить список кнопок формы
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetButtonByForm(int idForm)
        {
            return Tools.CreateResult(true, "", await _reportManager.GetButtonsAsync(GetUserName(), idForm, true));
        }

        /// <summary>
        /// Получить список объектов валидации для сущности
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetValidationRule(int idForm)
        {
            return Tools.CreateResult(true, "", await _reportManager.GetValidationRuleEntity(idForm));
        }

        /// <summary>
        /// Получить форму для редактирования
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetFormModelByEdit(int id)
        {
            return Tools.CreateResult(true, "", await _reportManager.GetFormModel(id));
        }

        /// <summary>
        /// Сохранить модель данных
        /// </summary>
        /// <returns>Модель для сохранения</returns>
        [HttpPost]
        public async Task<IActionResult> SaveFormModel([FromBody] FormModelDto formModel)
        {
            return Tools.CreateResult(true, "", await _reportManager.AddOrEditFormReportAsync(formModel));
        }

        /// <summary>
        /// Сохранить правило валидации данных
        /// </summary>
        /// <returns>Модель для сохранения</returns>
        [HttpPost]
        public async Task<IActionResult> SaveValidationModel([FromBody] ValidationRuleEntityDto formModel)
        {
            return Tools.CreateResult(true, "", await _reportManager.AddValidationRule(formModel));
        }

        /// <summary>
        /// Сохранить модель данных
        /// </summary>
        /// <returns>Модель для сохранения</returns>
        [HttpPost]
        public async Task<IActionResult> SaveFieldByForm([FromBody] FieldAccessDto model)
        {
            return Tools.CreateResult(true, "", await _reportManager.AddOrEditFieldAsync(model));
        }

        /// <summary>
        /// Сохранить модель данных
        /// </summary>
        /// <returns>Модель для сохранения</returns>
        [HttpPost]
        public async Task<IActionResult> SaveButtonByForm([FromBody] ButtonAccessDto model)
        {
            return Tools.CreateResult(true, "", await _reportManager.AddOrEditButtonAsync(model));
        }

        /// <summary>
        /// Удалить модель 
        /// </summary>
        /// <returns>Id модели которую нужно удалить</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveFormModel(int id)
        {
            return Tools.CreateResult(true, "", await _reportManager.RemoveFormReportAsync(id));
        }

        /// <summary>
        /// Удалить правило валидации
        /// </summary>
        /// <returns>Id модели которую нужно удалить</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveValidationRule(int id)
        {
            return Tools.CreateResult(true, "", await _reportManager.RemoveValidationRule(id));
        }

        /// <summary>
        /// Удалить модель поля
        /// </summary>
        /// <returns>Id модели которую нужно удалить</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveFieldModel(int id)
        {
            return Tools.CreateResult(true, "", await _reportManager.RemoveFieldAsync(id));
        }

        /// <summary>
        /// Удалить модель кнопки
        /// </summary>
        /// <returns>Id модели которую нужно удалить</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveButtonModel(int id)
        {
            return Tools.CreateResult(true, "", await _reportManager.RemoveButtonAsync(id));
        }

        /// <summary>
        /// Получить список типов форм
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetTypeForms()
        {
            return Tools.CreateResult(true, "", await _reportManager.GetTypeForms());
        }

        /// <summary>
        /// Генерация полей из схемы БД
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GenerateFiledsMySql(int formId)
        {
            await _generateField.GenerateField(formId);
            return Tools.CreateResult(true, "", true);
        }

        /// <summary>
        /// Генерация стандартных кнопок
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GenerateButtonMySql(int formId)
        {
            await _generateField.GenerateDefaultBtn(formId);
            return Tools.CreateResult(true, "", true);
        }

        /// <summary>
        /// Добавление или редактирование типов форм
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddTypeForm([FromBody]TypeForm typeForm)
        {
            var result = await _reportManager.AddOrEditTypeForm(typeForm);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Удаление типа формы
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveTypeForm(int modelId)
        {
            var result = await _reportManager.RemoveTypeFormAsync(modelId);
            return Tools.CreateResult(true, "", result);
        }


        /// <summary>
        /// Получение дерева моделей
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GetSolutionModel()
        {
            var result = await _reportManager.GetSolutionModel(GetUserName(), true);
            return Tools.CreateResult(true, "", result);
        }





    }
}
