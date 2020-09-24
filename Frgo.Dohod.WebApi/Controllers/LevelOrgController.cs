using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Interfaces.Adapters.Settings;
using Frgo.Dohod.DbModel.ModelDto.LevelOrg;
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
    public class LevelOrgController : BaseController
    {
        ILevelOrgAdapter _levelOrg;
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"LevelOrgController";

        public LevelOrgController(ILevelOrgAdapter levelOrg) : base(ModuleName)
        {
            _levelOrg = levelOrg;
        }
        /// <summary>
        /// добавление записи в t_levelorg
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AddLevelOrg(LevelOrgAddArgs args)
        {
            return Tools.CreateResult(true, "",await _levelOrg.AddAsync(args));
        }
        /// <summary>
        /// удаление записи в t_levelorg
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Remove(int idOrg)
        {
            return Tools.CreateResult(true, "",await _levelOrg.RemoveAsync(idOrg));
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
        public async Task<IActionResult> GetAll()
        {
            return Tools.CreateResult(true, "",await _levelOrg.GetAll());
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
        public async Task<IActionResult> Edit([FromBody] LevelOrgDto levelOrg)
        {
            return Tools.CreateResult(true, "",await _levelOrg.EditLevelOrgAsync(levelOrg));
        }
    }
}
