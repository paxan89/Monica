using System.Threading.Tasks;
using Monica.Core.DbModel.ModelsAuth;

namespace Monica.Core.Abstraction.Authorize
{
    /// <summary>
    /// Адаптер для авторизации пользователей в системе
    /// </summary>
    public interface IMonicaAuthorizeDataAdapter
    {
        /// <summary>
        /// Авторизация пользователя в системе. При успешном входе пользователю будет отправлятся авторизационной ключ 
        /// </summary>
        /// <param name="userAuthArgs">Модель параметров для входа</param>
        /// <returns>JWT Token</returns>
        Task<TokenDto> LoginAsync(UserAuthArgs userAuthArgs);


        /// <summary>
        /// Обновить токен авторизации, когда он подойдет к концу действия
        /// </summary>
        /// <param name="refreshToken">Токен</param>
        /// <returns>Новый токен авторизации</returns>
        Task<TokenDto> RefreshTokenAsunc(string refreshToken);
    }
}
