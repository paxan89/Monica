using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelDto;
using Frgo.Dohod.DbModel.ModelDto.LevelOrg;
using Frgo.Dohod.DbModel.ModelDto.Roles;
using Frgo.Dohod.DbModel.ModelDto.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Frgo.Dohod.DbModel.Interfaces.Adapters.Settings
{
    public interface IRolesAdapter
    {
        /// <summary>
        /// Метод возвращающий роли для организаций
        /// </summary>
        /// <param name="levelOrg"></param>
        /// <returns></returns>
        Task<IEnumerable<UserRoleDto>> GetRolesByLevelOrgAsync(int idOrg);
        /// <summary>
        /// Метод добавления роли для выбранной организации
        /// </summary>
        /// <param name="levelOrg"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<ResultCrmDb> AddRoleForLevelOrgAsync(RoleCreateArgs args);
        /// <summary>
        /// Метод изменения названия выбранной роли
        /// </summary>
        /// <param name="sysIdRole"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        Task<ResultCrmDb> EditUserRoleAsync(int sysIdRole, string newName);
        /// <summary>
        /// Метод удаления выбранной роли
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<ResultCrmDb> RemoveUserRoleAsync(int idRole);
        /// <summary>
        /// удаление связи  между ролью и пользователем
        /// </summary>
        Task<ResultCrmDb> RemoveUserFromRoleAsync(int idUser, int idRole);

        Task<ResultCrmDb> AddUserToRoleAsync(RoleLinkArgs args); //(IEnumerable<int> idUser, int idRole);
    }
}
