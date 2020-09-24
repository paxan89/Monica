using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.ModelParametrs.ModelsArgs;

namespace Monica.Core.Abstraction.Registration
{
    /// <summary>
    /// Регистрация пользователей в системе
    /// </summary>
    public interface IRegistrationUserAdapter
    {
        /// <summary>
        /// Подтверждение EMail
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<IActionResult> ConfirmEmail(string userId, string code);
        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IActionResult> Register(RegistrationUserArgs model);
        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IActionResult> RegisterAndSetPassword(RegistrationUserArgs model);
        /// <summary>
        /// Смена пароля текущим пользователем
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Новый пароль</param>
        /// <param name="oldPassword">Старый пароль</param>
        /// <returns></returns>
        Task<IActionResult> ChangePassword(string username, string password, string oldPassword);

        /// <summary>
        /// Выслать код восстановления по e-mail
        /// </summary>
        /// <param name="email">e-mail</param>
        /// <param name="userName">Аккоунт пользователя</param>
        /// <param name="html"></param>
        /// <returns></returns>
        Task<IActionResult> ForgotPassword(string email, string userName, string html);
        /// <summary>
        /// Восстановить пароль по коду безопасности и e-mail
        /// </summary>
        /// <param name="email">Входная модель</param>
        /// <param name="code">Код безопасности</param>
        /// <param name="newPassword">Новый пароль</param>
        /// <returns></returns>
        Task<IActionResult> ChangePasswordBySeccode(string email, string code, string newPassword);
        /// <summary>
        /// Удаление пользователя. Нужен после выполнения теста
        /// </summary>
        /// <param name="nameUser"></param>
        /// <returns></returns>
        Task RemoveUser(string nameUser);
        /// <summary>
        /// Отправить подтверждение email повторно
        /// </summary>
        /// <param name="email">Адрес электронной почты</param>
        /// <returns>Результат</returns>
        Task<IActionResult> SendConfirmEmail(string email);
    }
}
