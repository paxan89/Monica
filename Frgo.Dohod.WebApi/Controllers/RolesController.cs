using System.Collections.Generic;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Interfaces.Adapters.Settings;
using Frgo.Dohod.DbModel.ModelDto.LevelOrg;
using Frgo.Dohod.DbModel.ModelDto.Roles;
using Frgo.Dohod.DbModel.ModelDto.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Controllers;

namespace Frgo.Dohod.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RolesController : BaseController
    {
        IRolesAdapter _roles;
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"RolesController";
        public RolesController(IRolesAdapter roles) : base(ModuleName)
        {
            this._roles = roles;
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AddRole(RoleCreateArgs args)
        {
            return Tools.CreateResult(true, "", await _roles.AddRoleForLevelOrgAsync(args));
        }
        /// <summary>
        /// Получить список всех ролей организации
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetRolesByOrg(int idOrg)
        {
            return Tools.CreateResult(true, "", await _roles.GetRolesByLevelOrgAsync(idOrg));
        }
        /// <summary>
        /// удаление роли для организации
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Remove(int idRole)
        {
            return Tools.CreateResult(true, "", await _roles.RemoveUserRoleAsync(idRole));
        }
        /// <summary>
        /// Изменить роль
        /// </summary>
        /// <param name="idRole">Системный номер записи пользователя</param>
        /// <param name="newName">Новое название роли</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Edit(int idRole, string newName)
        {
            return Tools.CreateResult(true, "", await _roles.EditUserRoleAsync(idRole,newName));
        }
        /// <summary>
        /// Добавить пользователя к роли
        /// </summary>
        /// <param name="idUser">Системный номер записи пользователя</param>
        /// <param name="idRole">Системный номер записи роли</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AddUserToRole(RoleLinkArgs args)//(IEnumerable<int> idUser, int idRole)
        {
            return Tools.CreateResult(true, "", await _roles.AddUserToRoleAsync( args)); //(idUser,idRole));
        }
        /// <summary>
        /// Удаление связи  между ролью и пользователем
        /// </summary>
        /// <param name="idUser">Системный номер записи пользователя</param>
        /// <param name="idRole">Системный номер записи роли</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> RemoveUserFromRole(int idUser, int idRole)
        {
            return Tools.CreateResult(true, "", await _roles.RemoveUserFromRoleAsync(idUser,idRole ));
        }
    }
}
