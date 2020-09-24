using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Profile;
using Monica.Core.DbModel.IdentityModel;
using Monica.Core.ModelParametrs.ModelsArgs;
using Newtonsoft.Json;

namespace Monica.Core.Controllers.Crm
{
    /// <summary>
    /// Основной контроллер для авторизации пользователей в системе
    /// </summary>
    [Authorize(Roles = "FunctionalAdministrator")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class AdministrationController : BaseController
    {

        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"Administration";

        private readonly IManagerProfile _managerProfile;
        private readonly RoleManager<ApplicationRole> _roleManager;


        public AdministrationController(IManagerProfile managerProfile, RoleManager<ApplicationRole> roleManager) : base(ModuleName)
        {
            _managerProfile = managerProfile;
            _roleManager = roleManager;
        }


        /// <summary>
        /// Изменить данные профиля
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> EditProfile([FromBody] RegistrationUserArgs registration)
        {
            var result = await _managerProfile.EditProfile(registration, GetUserName());
            if (!result.Succeeded)
            {
                return new ObjectResult(JsonConvert.SerializeObject(result.Errors)) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            return Tools.CreateResult(true, "","");
        }


        /// <summary>
        /// Сделать профиль не активным. Доступно только через CRM
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> DeleteProfile(string userName)
        {
            var result = await _managerProfile.DeleteProfile(userName);
            if (!result.Succeeded)
            {
                return new ObjectResult(JsonConvert.SerializeObject(result.Errors)) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            return Tools.CreateResult(true, "", "");
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
        public async Task<IActionResult> GetListUsers([FromBody] UserArgs userArgs)
        {
            var result = await _managerProfile.GetListUsers(userArgs);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить список всех ролей пользователя системы
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetRolesByUser(string userName)
        {
            var result = await _managerProfile.GetRoleUsers(userName);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Добавить роль для пользователя
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AddToRoleByUser(string userName, string roleName)
        {
            var result = await _managerProfile.AddToRoleByUser(userName,roleName);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Удалить роль у пользователя
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> RemoveFromRoleByUser(string userName, string roleName)
        {
            var result = await _managerProfile.RemoveFromRoleByUser(userName, roleName);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Полное удаление пользователя из сервера авторизации и из CRM
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> FullDeleteProfile(string userName)
        {
            var result = await _managerProfile.FullDeleteProfile(userName);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Создать новую роль
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateRole(string nameRole)
        {
            var result = await _managerProfile.AddRoleAsync(nameRole);
            return Tools.CreateResult(true, "", result);
        }


        /// <summary>
        /// Удалить роль
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> RemoveRole(string nameRole)
        {
            var role = await _roleManager.FindByNameAsync(nameRole);
            if (role == null)
                return Tools.CreateResult(false, "", "Нет роли");
            var result = await _managerProfile.RemoveRole(nameRole);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить список всех ролей
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetRoles()
        {
            await _roleManager.Roles.LoadAsync();
            var roles = _roleManager.Roles.Select(s => s.Name);
            return Tools.CreateResult(true, "", roles);
        }

        /// <summary>
        /// Получить информацию о переданном пользователе
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUser(string userName)
        {
            return Tools.CreateResult(true, "", await _managerProfile.GetUser(userName));
        }

        /// <summary>
        /// Получить информацию о переданном пользователе
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetTypeUser()
        {
            return Tools.CreateResult(true, "", _managerProfile.GetTypeUsers());
        }

    }
}
