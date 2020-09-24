using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.ModelParametrs.ModelsArgs;
using Monica.Core.Paging;

namespace Monica.Core.Abstraction.Profile
{
    /// <summary>
    /// Менеджер для работы с профилями пользователей
    /// </summary>
    public interface IManagerProfile
    {
        /// <summary>
        /// Зарегистрировать пользователя в БД
        /// </summary>
        /// <returns></returns>
        Task<ResultCrmDb> RegistrationUser(RegistrationUserArgs registration, string userManager);
        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns></returns>
        Task RemoveUser(string userName);
        /// <summary>
        /// Редактирование учетных данных профиля
        /// </summary>
        /// <returns></returns>
        Task<ResultCrmDb> EditProfile(RegistrationUserArgs registrationUser, string userManager);
        /// <summary>
        /// Удаление данных профиля. Это не физическое удаление из таблицы, а только пометка на удаление
        /// </summary>
        /// <returns></returns>
        Task<ResultCrmDb> DeleteProfile(string userName);

        /// <summary>
        /// Получить список всех пользователей системы
        /// </summary>
        /// <returns></returns>
        Task<PagedResult<UserDto>> GetListUsers(UserArgs userArgs);
        /// <summary>
        /// Получить список ролей пользователя
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetRoleUsers(string userName);

        /// <summary>
        /// Сделать не активным профиль пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns></returns>
        Task<ResultCrmDb> FullDeleteProfile(string userName);

        /// <summary>
        /// Добавить роль для пользователя
        /// </summary>
        /// <returns></returns>
        Task<ResultCrmDb> AddToRoleByUser(string userName, string roleName);

        /// <summary>
        /// Убрать роль у пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="roleName">Название роли</param>
        /// <returns></returns>
        Task<ResultCrmDb> RemoveFromRoleByUser(string userName, string roleName);

        /// <summary>
        /// Получить информацию о переданном пользователе
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns>Модель пользователя</returns>
        Task<UserDto> GetUser(string userName);

        /// <summary>
        /// Получить список всех типов пользователей
        /// </summary>
        /// <returns></returns>
        IEnumerable<TypeUserDto> GetTypeUsers();

        /// <summary>
        /// Добавление новой роли
        /// </summary>
        /// <param name="roleName">имя роли</param>
        /// <returns></returns>
        Task<ResultCrmDb> AddRoleAsync(string roleName);

        /// <summary>
        /// Удаление роли 
        /// </summary>
        /// <param name="roleName">Имя роли</param>
        /// <returns></returns>
        Task<ResultCrmDb> RemoveRole(string roleName);
    }
}
