using System.Collections.Generic;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Interfaces.Adapters.Settings;
using Frgo.Dohod.DbModel.ModelDto;
using Frgo.Dohod.DbModel.ModelDto.LevelOrg;
using Frgo.Dohod.DbModel.ModelDto.Roles;
using Frgo.Dohod.DbModel.ModelDto.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Controllers;
using Monica.Core.ModelParametrs.ModelsArgs;

namespace Frgo.Dohod.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UsersController : BaseController
    {
        IUsersAdapter _users;
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"UsersController";
        public UsersController(IUsersAdapter users) : base(ModuleName)
        {
            this._users = users;
        }

        /// <summary>
        /// Получить всех пользователей связанных с ролью
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUsersByRole(int idRole)
        {
            return Tools.CreateResult(true, "", await _users.GetUsersByRoleAsync(idRole));
        }
        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUsers()
        {
            return Tools.CreateResult(true, "", await _users.GetUsersAsync());
        }
       
        /// <summary>
        /// Добавление нового пользователя
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> RegisterUser(RegistrationUserArgs args)
        {
            return Tools.CreateResult(true, "", await _users.RegisterUserAsync(args));
        }
        /// <summary>
        /// удаление пользователя 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> RemoveUserss(UsersRemoveArgs accUsers)
        {
            return Tools.CreateResult(true, "", await _users.RemoveUsersAsync(accUsers.Accounts));
        }
        ///// <summary>
        ///// Получить список всех пользователей системы
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[ProducesResponseType(200, Type = typeof(bool))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //[ProducesResponseType(403)]
        //public async Task<IActionResult> Edit([FromBody] LevelOrgDto levelOrg)
        //{
        //    return Tools.CreateResult(true, "", await _levelOrg.EditLevelOrgAsync(levelOrg));
        //}
        //[HttpPost]
        //[ProducesResponseType(200, Type = typeof(bool))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //[ProducesResponseType(403)]
        //public async Task<IActionResult> GetRolesByLevelOrg(int idLevelOrg)
        //{
        //    return Tools.CreateResult(true, "", await _levelOrg.GetRolesByLevelOrgAsync(idLevelOrg));
        //}


    }
}
