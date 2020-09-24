using Frgo.Dohod.DbModel.Interfaces.Adapters.Settings;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Data;
using Frgo.Dohod.DbModel.ModelCrm.Profile;
using Frgo.Dohod.DbModel.ModelDto;
using Frgo.Dohod.DbModel.ModelDto.LevelOrg;
using Frgo.Dohod.DbModel.ModelDto.Roles;
using Frgo.Dohod.DbModel.ModelDto.Users;
using Microsoft.EntityFrameworkCore;
using Monica.CrmDbModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frgo.Dohod.DbModel.DataAdapter.Settings
{
    /// <summary>
    /// Адаптер для работы с ролями пользователей
    /// </summary>
    public class RolesAdapter : IRolesAdapter
    {
        private DohodDbContext _crmDbContext;
        public RolesAdapter(DohodDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<IEnumerable<UserRoleDto>> GetRolesByLevelOrgAsync(int idOrg)
        {
            var result = new List<UserRoleDto>();
            try
            {
                var roles = _crmDbContext.userrole.Where(s => s.LevelOrgId == idOrg);
                if (roles.Count() == 0)
                    throw new Exception($"В  отсутствуют роли.");//{levelOrg.Caption}
                foreach (var role in roles)
                {
                    result.Add(role.Map(new UserRoleDto()));
                }
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
        public async Task<ResultCrmDb> AddRoleForLevelOrgAsync(RoleCreateArgs args)
        {
            var result = new ResultCrmDb();
            try
            {
                var role = new userrole();
                role.LevelOrgId = args.IdLevelorg;
                role.Name = args.CaptionRole;
                role.Sysname = args.CaptionRole;
                await _crmDbContext.userrole.AddAsync(role);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
        public async Task<ResultCrmDb> RemoveUserRoleAsync(int idRole)
        {
            var result = new ResultCrmDb();
            try
            {
                _crmDbContext.userlinkrole.RemoveRange(_crmDbContext.userlinkrole.Where(l => l.UserRoleId == idRole));
                _crmDbContext.userrole.Remove(await _crmDbContext.userrole.FirstOrDefaultAsync(ur => ur.Id == idRole));
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
        public async Task<ResultCrmDb> EditUserRoleAsync(int sysIdRole, string newName)
        {
            var result = new ResultCrmDb();
            try
            {
                var role = await _crmDbContext.userrole.FirstOrDefaultAsync(ur => ur.Id == sysIdRole);
                role.Name = newName;
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
        public async Task<ResultCrmDb> AddUserToRoleAsync(RoleLinkArgs args)//(IEnumerable<int> idUser, int idRole)
        {
            var result = new ResultCrmDb();
            try
            {
                var link = new List<UserLinkRole>();
                foreach (var id in args.IdUsers)//idUser)
                {
                    var add = await _crmDbContext.userlinkrole.FirstOrDefaultAsync(l => l.UserRoleId == args.IdRole & l.UserId == id);
                    if (add != null)
                        throw new Exception("К роли уже добавлен данный пользователь.");
                    link.Add(new UserLinkRole() { UserId = id, UserRoleId = args.IdRole });
                }
                await _crmDbContext.userlinkrole.AddRangeAsync(link);// AddAsync(link);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
        public async Task<ResultCrmDb> RemoveUserFromRoleAsync(int idUser, int idRole)
        {
            var result = new ResultCrmDb();
            try
            {
                var removeable = await _crmDbContext.userlinkrole.FirstOrDefaultAsync(l => l.UserId == idUser & l.UserRoleId == idRole);
                if (removeable == null)
                    throw new Exception("Не обнаружена связь между пользователем и ролью.");
                _crmDbContext.userlinkrole.Remove(removeable);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
    }
}
