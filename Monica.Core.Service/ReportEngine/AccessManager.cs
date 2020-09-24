using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.EngineReport;

namespace Monica.Core.Service.ReportEngine
{
    /// <summary>
    /// Менеджер получения доступа на элементы. Так же тут будет отображаться менеджер профиля
    /// </summary>
    public class AccessManager : IAccessManager
    {
        private ReportDbContext _reportDbContext;

        public AccessManager(ReportDbContext reportDbContext)
        {
            _reportDbContext = reportDbContext;
        }

        /// <summary>
        /// Получить список доступов для режима
        /// </summary>
        /// <param name="userName">Имя пользвателя</param>
        /// <returns>Список доступных режимов. Если нет то считается что для формы заложена видимость всегда</returns>
        public async Task<IEnumerable<AccessForm>> GetAccessFormsAsync(string userName)
        {
            await _reportDbContext.UserLinkRole.LoadAsync();
            var query = (await _reportDbContext.AccessForm.Where(f => f.FormModelId != null && f.FieldId == null && f.ButtonFormId == null).Join(
                _reportDbContext.UserLinkRole.Where(f => f.User.Account == userName).Select(s => s.UserRole),
                form => form.UserRoleId,
                role => role.Id, (form, role) => form).ToListAsync()).GroupBy(g => new {g.FormModel}).Select(s =>
                s.OrderByDescending(o => (int) o.TypeAccec).FirstOrDefault());
            return query;
        }

        /// <summary>
        /// Получить список профилей для режима
        /// </summary>
        /// <param name="userName">Пользователь</param>
        /// <param name="formId">Режим</param>
        /// <returns>Список профиля</returns>
        public async Task<IEnumerable<ProfileForm>> GetPropfileForm(string userName, int formId)
        {
            await _reportDbContext.UserLinkRole.LoadAsync();
            var query = (await _reportDbContext.ProfileForm.Where(f => f.FormModelId == formId).Join(
                _reportDbContext.UserLinkRole.Where(f => f.User.Account == userName).Select(s => s.UserRole),
                form => form.UserRoleId,
                role => role.Id, (form, role) => form).ToListAsync()).GroupBy(g => new { g.FormModel }).Select(s =>
                s.OrderByDescending(o => (int)o.TypeProfileForm).FirstOrDefault());
            return query;
        }
        
        /// <summary>
        /// Получить доступ на один режим. Если ничего не пришло, то на форму никто ничего не прописал из разрешений
        /// </summary>
        /// <param name="userName">Имя пользвателя</param>
        /// <param name="formModelId">Ссылка на форму</param>
        /// <returns>Список доступных режимов. Если нет то считается что для формы заложена видимость всегда</returns>
        public async Task<AccessForm> GetAccessFormAsync(string userName, int formModelId)
        {
            await _reportDbContext.UserLinkRole.LoadAsync();
            var query = (await _reportDbContext.AccessForm.Where(f => f.FormModelId == formModelId && f.FieldId == null && f.ButtonFormId == null).Join(
                _reportDbContext.UserLinkRole.Where(f => f.User.Account == userName).Select(s => s.UserRole),
                form => form.UserRoleId,
                role => role.Id, (form, role) => form).ToListAsync()).GroupBy(g => new { g.FormModel }).Select(s =>
                s.OrderByDescending(o => (int)o.TypeAccec).FirstOrDefault());
            return query.FirstOrDefault();
        }

        /// <summary>
        /// Получить список доступов для режима
        /// </summary>
        /// <param name="userName">Имя пользвателя</param>
        /// <param name="formId">Уникальный идентификатор формы</param>
        /// <returns>Список доступных режимов. Если нет то считается что для формы заложена видимость всегда</returns>
        public async Task<IEnumerable<AccessForm>> GetAccessFildsByFormAsync(string userName, int formId)
        {
            await _reportDbContext.UserLinkRole.LoadAsync();
            var query = (await _reportDbContext.AccessForm.Where(f => f.FormModelId == formId && f.FieldId != null && f.ButtonFormId == null).Join(
                _reportDbContext.UserLinkRole.Where(f => f.User.Account == userName),
                form => form.UserRoleId,
                role => role.Id, (form, role) => form).ToListAsync()).GroupBy(g => new { g.FieldId }).Select(s =>
                s.OrderByDescending(o => (int)o.TypeAccec).FirstOrDefault());
            return query;
        }

        /// <summary>
        /// Получить список доступов для режима
        /// </summary>
        /// <param name="userName">Имя пользвателя</param>
        /// <param name="formId">Уникальный идентификатор формы</param>
        /// <returns>Список доступных режимов. Если нет то считается что для формы заложена видимость всегда</returns>
        public async Task<IEnumerable<AccessForm>> GetAccessButtonByFormAsync(string userName, int formId)
        {
            await _reportDbContext.UserLinkRole.LoadAsync();
            var query = (await _reportDbContext.AccessForm.Where(f => f.FormModelId == formId && f.FieldId == null && f.ButtonFormId != null).Join(
                _reportDbContext.UserLinkRole.Where(f => f.User.Account == userName).Select(s => s.UserRole),
                form => form.UserRoleId,
                role => role.Id, (form, role) => form).ToListAsync()).GroupBy(g => new { g.ButtonFormId }).Select(s =>
                s.OrderByDescending(o => (int)o.TypeAccec).FirstOrDefault());
            return query;
        }
    }
}
