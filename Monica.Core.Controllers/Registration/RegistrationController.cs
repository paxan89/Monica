using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Abstraction.Profile;
using Monica.Core.Abstraction.Registration;
using Monica.Core.ModelParametrs.ModelsArgs;
using Newtonsoft.Json;

namespace Monica.Core.Controllers.Registration
{
    /// <summary>
    /// Методы регистрации пользователя в системе
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class RegistrationController : BaseController
    {
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"Registration";

        private readonly IRegistrationUserAdapter _registrationUserAdapter;
        private readonly IManagerProfile _managerProfile;

        /// <summary>
        /// Конструктор
        /// </summary>
        public RegistrationController(IRegistrationUserAdapter registrationUserAdapter, IManagerProfile managerProfile) : base(ModuleName)
        {
            _registrationUserAdapter = registrationUserAdapter;
            _managerProfile = managerProfile;
        }

        /// <summary>
        /// Регистрация пользователя в системе
        /// </summary>
        /// <param name="registration">Модель для регистрации пользователя</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegistrationUser([FromBody] RegistrationUserArgs registration)
        {
            return await _registrationUserAdapter.Register(registration);
        }

        /// <summary>
        /// Регистрация пользователя в системе c установкой пароля его
        /// </summary>
        /// <param name="registration">Модель для регистрации пользователя</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegistrationUserAndSetPassword([FromBody] RegistrationUserArgs registration)
        {
            return await _registrationUserAdapter.RegisterAndSetPassword(registration);
        }

        /// <summary>
        /// Изменить данные профиля. Работает только для пользователя, под которым зарегистрировались.
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditProfile([FromBody] RegistrationUserArgs registration)
        {
            if (registration.Account != GetUserName())
                return new ObjectResult("Нет доступа") { StatusCode = 403 };
            var result = await _managerProfile.EditProfile(registration, null);
            if (!result.Succeeded)
            {
                return new ObjectResult(JsonConvert.SerializeObject(result.Errors)) { StatusCode =  StatusCodes.Status500InternalServerError};
            }
            return Json(true);
        }

        /// <summary>
        /// Получить информацию о профиле зарегистрированного пользователя
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetMyProfile()
        {
            var userName = GetUserName();
            if (string.IsNullOrEmpty(userName))
                return new ObjectResult("Нет доступа") { StatusCode = 403};
            var result = await _managerProfile.GetUser(userName);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Смена пароля пользователя
        /// </summary>
        /// <param name="model">Модель параметров</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordArgs model)
        {
            var account = GetUserName();
            return await _registrationUserAdapter.ChangePassword(account, model.NewPassword, model.CurrentPassword);
        }

        /// <summary>
        /// Выслать сообщение о восстановлении пароля по секретному коду
        /// </summary>
        /// <param name="model">Модель параметров</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordArgs model)
        {
            return await _registrationUserAdapter.ForgotPassword(model.Email,model.UserName, null);
        }

        /// <summary>
        /// Подтверждение адреса электронно почты пользователя
        /// </summary>
        /// <param name="model">Модель параметров</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmModelArgs model)
        {
            return await _registrationUserAdapter.ConfirmEmail(model.Email, model.Code);
        }

        /// <summary>
        /// Выслать письмо с кодом подтверждения повторно
        /// </summary>
        /// <param name="email">Электронный адрес пользователя</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendConfirmEmail(string email)
        {
            return await _registrationUserAdapter.SendConfirmEmail(email);
        }

        /// <summary>
        /// Восстановить пароль по секретному коду
        /// </summary>
        /// <param name="model">Модель параметров</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePasswordBySeccode([FromBody] ResetPasswordArgs model)
        {
            return await _registrationUserAdapter.ChangePasswordBySeccode(model.Email, model.Code, model.NewPassword);
        }


    }
}
