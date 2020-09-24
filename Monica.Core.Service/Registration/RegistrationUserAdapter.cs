using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Monica.Core.Abstraction.MailKit;
using Monica.Core.Abstraction.Profile;
using Monica.Core.Abstraction.Registration;
using Monica.Core.Controllers;
using Monica.Core.DbModel.IdentityModel;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.Exceptions;
using Monica.Core.ModelParametrs.ModelsArgs;
using Newtonsoft.Json;

namespace Monica.Core.Service.Registration
{
    /// <summary>
    /// Работа с регистрацией пользователей в системе
    /// </summary>
    public class RegistrationUserAdapter : IRegistrationUserAdapter
    {
        private readonly IEmailService _emailService;
        private readonly IManagerProfile _managerProfile;
        private IConfiguration _configuration;
        private ILogger _logger;

        /// <summary>
        /// Управление пользователями
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Конструктор
        /// </summary>
        public RegistrationUserAdapter(UserManager<ApplicationUser> userManager, IEmailService emailService, IManagerProfile managerProfile, ILogger logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _managerProfile = managerProfile;
            _logger = logger;
            var confFileName = Path.Combine(
                Path.GetDirectoryName(GetType().Assembly.Location),
                $"RegistrationService.config");
            var build = new ConfigurationBuilder().AddXmlFile(confFileName);
            _configuration = build.Build();
        }

        /// <summary>
        /// Подтверждение EMail
        /// </summary>
        /// <param name="userId">Email пользователя</param>
        /// <param name="code">Уникальный код высланный пользователю для подтверждения адреса пользователем</param>
        /// <returns></returns>
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                throw new NonUserException();
            }

            var user = await _userManager.FindByEmailAsync(userId);
            if (user == null)
            {
                throw new NonUserException();
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
                throw new UserMessageException("Error confrim email adress");
            return new JsonResult("ConfirmEmail", JsonSupport.JsonSerializerSettings);
        }

        /// <summary>
        /// Отправить подтверждение email повторно
        /// </summary>
        /// <param name="email">Адрес электронной почты</param>
        /// <returns>Результат</returns>
        public async Task<IActionResult> SendConfirmEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Tools.CreateResult(true, "", true);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string codeurl = HttpUtility.UrlEncode(code);
            return await SentResetPasswordEmail(user.Email, codeurl, "ConfirmEmail");
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="model">Модель параметров для регистрации</param>
        /// <returns></returns>
        public async Task<IActionResult> Register(RegistrationUserArgs model)
        {
            var user = new ApplicationUser { UserName = model.Account, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return Tools.CreateResult(false, "Ошибка регистрации пользователя", result);

            result = await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, user.UserName));
            if (!result.Succeeded) return Tools.CreateResult(false, "Ошибка регистрации пользователя", result);

            var resultCrm = await _managerProfile.RegistrationUser(model, null);

            if (!resultCrm.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return Tools.CreateResult(false, "Ошибка регистрации пользователя", resultCrm);
                //throw new UserMessageException(JsonConvert.SerializeObject(resultCrm.Errors));
            }
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string codeurl = HttpUtility.UrlEncode(code);
            try
            {
                await SentResetPasswordEmail(user.Email, codeurl, "ConfirmEmail");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.LogError(e, e.Message);
            }

            return Tools.CreateResult(true, "", new ResultCrmDb { Succeeded = true});
        }

        /// <summary>
        /// Регистрация нового пользователя c с установкой им пароля
        /// </summary>
        /// <param name="model">Модель параметров для регистрации</param>
        /// <returns></returns>
        public async Task<IActionResult> RegisterAndSetPassword(RegistrationUserArgs model)
        {
            var user = new ApplicationUser { UserName = model.Account, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return Tools.CreateResult(false, "Ошибка регистрации пользователя", result);

            result = await _userManager.AddClaimAsync(user, new Claim("given_name", user.UserName));
            if (!result.Succeeded)
                return Tools.CreateResult(false, "Ошибка регистрации пользователя", result); 

            var resultCrm = await _managerProfile.RegistrationUser(model, null);
            if (!resultCrm.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return Tools.CreateResult(false, "Ошибка регистрации пользователя", resultCrm);
            }
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await ConfirmEmail(model.Account, code);
            return Tools.CreateResult(true, "", new ResultCrmDb { Succeeded = true });
        }

        /// <summary>
        /// Смена пароля текущим пользователем
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Новый пароль</param>
        /// <param name="oldPassword">Старый пароль</param>
        /// <returns></returns>
        public async Task<IActionResult> ChangePassword(string username, string password, string oldPassword)
        {
            try
            {
                ApplicationUser applicationUser = await _userManager.FindByNameAsync(username);
                if (!(applicationUser is null))
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(applicationUser, oldPassword, password);
                    if (!changePasswordResult.Succeeded)
                    {
                        return new ObjectResult(JsonConvert.SerializeObject(changePasswordResult.Errors))
                        { StatusCode = StatusCodes.Status500InternalServerError };
                    }

                    // Все получилось
                    return Tools.CreateResult(true, "", new ResultCrmDb { Succeeded = true });
                }

                throw new UserNameAlreadyExistsException(username + @" - пользователь с таким логином не найден в базе данных!", null);

            }
            catch (Exception e)
            {
                return new ObjectResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        /// <summary>
        /// Выслать код восстановления по e-mail
        /// </summary>
        /// <param name="email">e-mail</param>
        /// <param name="userName"></param>
        /// <param name="html">Тело письма для которого высылается секретный код для восстановления или задания пароля</param>
        /// <returns></returns>
        public async Task<IActionResult> ForgotPassword(string email, string userName, string html)
        {
            html = string.IsNullOrWhiteSpace(html) ? "HtmlResetPassword" : html;
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Tools.CreateResult(true, "", new ResultCrmDb { Succeeded = true });
            // пользователь с данным email может отсутствовать в бд
            // тем не менее мы выводим стандартное сообщение, чтобы скрыть 
            // наличие или отсутствие пользователя в бд
            //throw new UserNameAlreadyExistsException(@"Пользователь с таким e-mail не найден!", null);

            //Проверим на логин пользователя. Если нет то игнорируем выполнение
            user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return Tools.CreateResult(true, "", new ResultCrmDb { Succeeded = true });

            if (!(await _userManager.IsEmailConfirmedAsync(user)))
                // Не подтверждённый e-mail
                throw new UserNameAlreadyExistsException(@"У пользователя не подтвержден e-mail, сброс пароля не возможен!", null);


            string code = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Перекодируем code
            string codeurl = HttpUtility.UrlEncode(code);

            return await SentResetPasswordEmail(email, codeurl, html);
        }

        /// <summary>
        /// Восстановить пароль по коду безопасности и e-mail
        /// </summary>
        /// <param name="email">Входная модель</param>
        /// <param name="code">Код безопасности</param>
        /// <param name="newPassword">Новый пароль</param>
        /// <returns></returns>
        public async Task<IActionResult> ChangePasswordBySeccode(string email, string code, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ObjectResult(@"Пользователь с таким e-mail не найден!") { StatusCode = StatusCodes.Status404NotFound };

            if (!(await _userManager.IsEmailConfirmedAsync(user)))
                // Не подтверждённый e-mail
                return new ObjectResult(@"У пользователя не подтвержден e-mail, сброс пароля не возможен!") { StatusCode = StatusCodes.Status404NotFound };

            string encodedCode = HttpUtility.UrlDecode(code);
            var result = await _userManager.ResetPasswordAsync(user, encodedCode, newPassword);

            if (result.Succeeded)
            {

                return new JsonResult(true, JsonSupport.JsonSerializerSettings);
            }

            // Есть ошибки
            return new ObjectResult(JsonConvert.SerializeObject(result.Errors)) { StatusCode = StatusCodes.Status500InternalServerError };
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="nameUser">Имя пользователя</param>
        /// <returns></returns>
        public async Task RemoveUser(string nameUser)
        {
            await _managerProfile.RemoveUser(nameUser);
            var user = await _userManager.FindByNameAsync(nameUser);
            if (user == null)
                return;
            await _userManager.DeleteAsync(user);
        }


        /// <summary>
        /// Отправка письма для сброса пароля
        /// </summary>
        /// <param name="email">e-mail</param>
        /// <param name="codeurl">код для сброса UrlEncode</param>
        /// <param name="typeHtml"></param>
        /// <returns></returns>
        private async Task<IActionResult> SentResetPasswordEmail(string email, string codeurl, string typeHtml)
        {
            var htmlName = _configuration[$"{typeHtml}:HtmlFile"];
            var htmlSubject = _configuration[$"{typeHtml}:Subject"];
            var htmlFileInfo = new FileInfo(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), htmlName));
            if (!htmlFileInfo.Exists)
                throw new UserMessageException("Не удалось определить файл для отправки Html");
            var html = File.ReadAllText(htmlFileInfo.FullName);
            // Парсим письмо (код восстановления и email)
            string parsedbody = html.Replace("{#code}", codeurl)
                .Replace("{#email}", email);
            // Дата время
            string date = DateTime.Now.ToString("dd.MM.yyyy");
            parsedbody = parsedbody.Replace("{#date}", date);
            
            string datetime = DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss");
            parsedbody = parsedbody.Replace("{#datetime}", datetime);
            
            await _emailService.SendAsync(email, string.Empty, string.Empty, htmlSubject, parsedbody, Encoding.UTF8, true);
            // Все получилось
            return Tools.CreateResult(true, "", new ResultCrmDb { Succeeded = true }); //new JsonResult(true, JsonSupport.JsonSerializerSettings);
        }


    }
}
