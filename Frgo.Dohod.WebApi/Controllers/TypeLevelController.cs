using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Interfaces.Adapters.Settings;
using Frgo.Dohod.DbModel.ModelDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Controllers;

namespace Frgo.Dohod.WebApi.Controllers
{
    /// <summary>
    /// Основной контроллер для взаимодействия с t_levelorg
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TypeLevelController : BaseController
    {
        ITypeLevelAdapter _typeLevel;
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"TypeLevelController";

        public TypeLevelController(ITypeLevelAdapter typeLevel) : base(ModuleName)
        {
            _typeLevel = typeLevel;
        }
        /// <summary>
        /// Получить список всех пользователей системы
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAllTypes()
        {
            return Tools.CreateResult(true, "",await _typeLevel.GetListTypeLevels());
        }
    }
}
