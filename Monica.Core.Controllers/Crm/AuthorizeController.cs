using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Abstraction.Authorize;
using Monica.Core.DbModel.ModelsAuth;
using Monica.Core.Exceptions;

namespace Monica.Core.Controllers.Crm
{
    /// <summary>
    /// Основной контроллер для авторизации пользователей в системе
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthorizeController : BaseController
    {
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"AuthorizeController";

        private readonly IMonicaAuthorizeDataAdapter _monicaAuthorizeDataAdapter;

        /// <summary>
        /// Конструктор
        /// </summary>
        public AuthorizeController(IMonicaAuthorizeDataAdapter monicaAuthorizeDataAdapter) : base(ModuleName)
        {
            _monicaAuthorizeDataAdapter = monicaAuthorizeDataAdapter;
        }

        /// <summary>
        /// Авторизация пользователя в системе через сервер авторизации
        /// </summary>
        /// <param name="userAuthArgs">Параметры пользователей</param>
        /// <returns>Токен</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserAuthArgs userAuthArgs)
        {
            try
            {
                return Tools.CreateResult(true, "", await _monicaAuthorizeDataAdapter.LoginAsync(userAuthArgs));
            }
            catch (Exception e)
            {
                if (e is UserMessageException)
                    return Unauthorized();
                throw;
            }
            
        }

        /// <summary>
        /// Обновить токен уже ранее зарегистрированный
        /// </summary>
        /// <param name="token">Токен</param>
        /// <returns>Новый токен</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(string token)
        {
            try
            {
                return Tools.CreateResult(true, "", await _monicaAuthorizeDataAdapter.RefreshTokenAsunc(token));
            }
            catch (Exception e)
            {
                if (e is UserMessageException)
                    return Unauthorized();
                throw;
            }
        }
    }
}
