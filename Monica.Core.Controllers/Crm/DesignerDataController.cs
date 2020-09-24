using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using Monica.Core.DbModel.ModelDto.Core;
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
    [Authorize]
    public class DesignerDataController : BaseController
    {
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"DesignerDataController";

        private readonly IGenerateField _generateField;
        private IReportData _reportData;
        private IReportManager _reportManager;

        /// <summary>
        /// Конструктор
        /// </summary>
        public DesignerDataController(IReportData reportData, IReportManager reportManager) : base(ModuleName)
        {
            _reportData = reportData;
            _reportManager = reportManager;
        }

        /// <summary>
        /// Получить список форм для редактирования
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetDataList([FromBody]BaseModelReportParam p)
        {
            p.UserName = GetUserName();
            var result = await _reportData.GetDataList(p);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить список кнопок
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetButtons([FromBody] BaseModelReportParam p)
        {
            var result = await _reportData.GetButtons(p.FormId, GetUserName());
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить список кнопок
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetButtonsDetail([FromBody] BaseModelReportParam p)
        {
            var result = await _reportData.GetButtonsDetail(p.FormId, GetUserName());
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получение дерева моделей
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GetSolutionModel()
        {
            var result = await _reportManager.GetSolutionModel(GetUserName(), false);
            return Tools.CreateResult(true, "", result);
        }
        /// <summary>
        /// Получить данные для формы редактирования
        /// </summary>
        /// <param name="id"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetEditModel(int id, int formId)
        {
            var result = await _reportData.GetEditModel(new BaseModelReportParam
                {FormId = formId, ModelId = id.ToString(), UserName = GetUserName()});
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить данные для формы редактирования
        /// </summary>
        /// <param name="id"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetCopyModel(int id, int formId)
        {
            var result = await _reportData.GetEditModel(new BaseModelReportParam
                { FormId = formId, ModelId = id.ToString(), UserName = GetUserName() });
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить данные для формы редактирования
        /// </summary>
        /// <param name="saveModel">Модель данных которую необходимо сохранить</param>
        /// <param name="formId"></param>
        /// <param name="typeEditor"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveModel([FromBody]dynamic saveModel, int formId, int typeEditor)
        {
            var result = await _reportData.SaveModel(saveModel, typeEditor,formId, GetUserName());
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить данные для формы редактирования
        /// </summary>
        /// <param name="key"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveEntity([FromBody] string[] key, int formId)
        {
            var result = await _reportData.RemoveEntity(formId, GetUserName(), key);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Проверить данные перед сохранением
        /// </summary>
        /// <param name="saveModel"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ValidateRuleEntity([FromBody] dynamic saveModel, int formId)
        {
            var result = await _reportData.ValidateModel(saveModel,formId);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить данные для детальной информации объекта
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDetailInfoModels(int formId)
        {
            var result = await _reportData.GetDetailInfoModels(formId, GetUserName());
            return Tools.CreateResult(true, "", result);
        }
    }
}
