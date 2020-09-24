using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.EngineReport;

namespace Monica.Core.Abstraction.ReportEngine
{
    /// <summary>
    /// Менеджер получения доступа на элементы. Так же тут будет отображаться менеджер профиля
    /// </summary>
    public interface IAccessManager
    {
        /// <summary>
        /// Получить список доступов для режима
        /// </summary>
        /// <param name="userName">Имя пользвателя</param>
        /// <returns>Список доступных режимов. Если нет то считается что для формы заложена видимость всегда</returns>
        Task<IEnumerable<AccessForm>> GetAccessFormsAsync(string userName);

        /// <summary>
        /// Получить список доступов для режима
        /// </summary>
        /// <param name="userName">Имя пользвателя</param>
        /// <param name="formId">Уникальный идентификатор формы</param>
        /// <returns>Список доступных режимов. Если нет то считается что для формы заложена видимость всегда</returns>
        Task<IEnumerable<AccessForm>> GetAccessFildsByFormAsync(string userName, int formId);

        /// <summary>
        /// Получить доступные кнопки для режимов доступов для режима
        /// </summary>
        /// <param name="userName">Имя пользвателя</param>
        /// <param name="formId">Уникальный идентификатор формы</param>
        /// <returns>Список доступных режимов. Если нет то считается что для формы заложена видимость всегда</returns>
        Task<IEnumerable<AccessForm>> GetAccessButtonByFormAsync(string userName, int formId);

        /// <summary>
        /// Получить доступ на один режим. Если ничего не пришло, то на форму никто ничего не прописал из разрешений
        /// </summary>
        /// <param name="userName">Имя пользвателя</param>
        /// <param name="formModelId">Ссылка на форму</param>
        /// <returns>Список доступных режимов. Если нет то считается что для формы заложена видимость всегда</returns>
        Task<AccessForm> GetAccessFormAsync(string userName, int formModelId);

        /// <summary>
        /// Получить список профилей для режима
        /// </summary>
        /// <param name="userName">Пользователь</param>
        /// <param name="formId">Режим</param>
        /// <returns>Список профиля</returns>
        Task<IEnumerable<ProfileForm>> GetPropfileForm(string userName, int formId);
    }
}
