using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Profile;
using Monica.Core.DbModel.IdentityModel;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.Profile;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.Exceptions;
using Monica.Core.ModelParametrs.ModelsArgs;
using Monica.Core.Paging;
using Monica.Core.Utils;

namespace Monica.Core.Service.Profile
{
    /// <summary>
    /// Менеджер для работы с профилем пользователя. Вынесены основные методы для добавления, удаления, редактирования пользователя
    /// </summary>
    public class ManagerProfile : IManagerProfile
    {
        private readonly ReportDbContext _crmDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ManagerProfile(ReportDbContext crmDbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _crmDbContext = crmDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Регистрация пользователя в БД CRM
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="userManager"></param>
        /// <returns></returns>
        public async Task<ResultCrmDb> RegistrationUser(RegistrationUserArgs registration, string userManager)
        {
            var result = new ResultCrmDb();
            var userManagerId = await _crmDbContext.User.Where(f => f.Account == userManager).FirstOrDefaultAsync();
            var typeUserId = await _crmDbContext.TypeUser.Where(s => s.Sysname == "PLAYER").FirstOrDefaultAsync();
            var countUser = await _crmDbContext.User.CountAsync(c => c.Account.ToLower() == registration.Account.ToLower());
            if (countUser > 0)
            {
                result.Succeeded = false;
                result.AddError("crm001", "Пользователь с таким логином уже существует");
                return result;
            }
            var user = new User();
            user.Account = registration.Account;
            user.Email = registration.Email;
            user.Name = registration.Name;
            user.Surname = registration.Surname;
            user.Middlename = registration.Middlename;
            user.TypeUser = typeUserId;
            _crmDbContext.User.Add(user);
            await _crmDbContext.SaveChangesAsync();
            result.Succeeded = true;
            return result;
        }

        public async Task RemoveUser(string userName)
        {
            var user = await _crmDbContext.User.FirstOrDefaultAsync(f => f.Account == userName);
            if (user == null)
                return;
            _crmDbContext.Remove(user);
            _crmDbContext.SaveChanges();
        }

        /// <summary>
        /// Редактировать профиль пользователя
        /// </summary>
        /// <param name="registrationUser">Данные, на которые нужно изменить профиль</param>
        /// <param name="userManager">Менеджер</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> EditProfile(RegistrationUserArgs registrationUser, string userManager)
        {
            var result = new ResultCrmDb();
            var userManagerId = userManager == null ? null : await _crmDbContext.User.Where(f => f.Account == userManager).FirstOrDefaultAsync();
            var user = await _crmDbContext.User.FirstOrDefaultAsync(f => f.Account == registrationUser.Account);
            if (user == null)
            {
                result.AddError("crm002", "Пользователя с таким аакаунтом не существует");
                result.Succeeded = false;
                return result;
            }

            user.Email = registrationUser.Email;
            user.Name = registrationUser.Name;
            user.Surname = registrationUser.Surname;
            user.Middlename = registrationUser.Middlename;
            user.Phone = registrationUser.Phone;
            _crmDbContext.Update(user);
            await _crmDbContext.SaveChangesAsync();
            result.Succeeded = true;
            return result;
        }

        /// <summary>
        /// Сделать не активным профиль пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> DeleteProfile(string userName)
        {
            var result = new ResultCrmDb();
            var user = await _crmDbContext.User.FirstOrDefaultAsync(f => f.Account == userName);
            if (user == null)
            {
                result.AddError("crm002", "Пользователя с таким аакаунтом не существует");
                result.Succeeded = false;
                return result;
            }

            user.IsDeleted = true;
            _crmDbContext.Update(user);
            _crmDbContext.SaveChanges();
            result.Succeeded = true;
            return result;
        }


        /// <summary>
        /// Сделать не активным профиль пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> FullDeleteProfile(string userName)
        {
            var result = new ResultCrmDb();
            var userIdentity = await _userManager.FindByNameAsync(userName);
            if (userIdentity != null)
                await _userManager.DeleteAsync(userIdentity);
            var user = await _crmDbContext.User.FirstOrDefaultAsync(f => f.Account == userName);
            if (user == null)
            {
                result.AddError("crm002", "Пользователя с таким акаунтом не существует");
                result.Succeeded = false;
                return result;
            }
            _crmDbContext.Remove(user);
            _crmDbContext.SaveChanges();
            result.Succeeded = true;
            return result;
        }

        /// <summary>
        /// Получить список всех пользователей системы
        /// </summary>
        /// <returns></returns>
        public async Task<PagedResult<UserDto>> GetListUsers(UserArgs userArgs)
        {
            await _crmDbContext.User.LoadAsync();
            var predicat = PredicateBuilder.New<User>(true);
            if (userArgs.DateBegin != null)
                predicat.And(user => user.CreateDate.Value >= userArgs.DateBegin);
            if (userArgs.DateEnd != null)
                predicat.And(user => user.CreateDate <= userArgs.DateEnd);
            if (!string.IsNullOrWhiteSpace(userArgs.Email))
                predicat.And(user => user.Email.Contains(userArgs.Email));
            if (userArgs.IsDeleted == true)
                predicat.And(user => user.IsDeleted == true);
            if (userArgs.IsDeleted == false)
                predicat.And(user => user.IsDeleted == false);
            if (!string.IsNullOrWhiteSpace(userArgs.Login))
                predicat.And(user => user.Account.Contains(userArgs.Login));
            if (!string.IsNullOrWhiteSpace(userArgs.FullName))
                predicat.And(user =>
                    $"{user.Surname} {user.Name} {user.Middlename}".Contains(userArgs.FullName));
            var result = _crmDbContext.User.Where(predicat).GetPaged(userArgs.Page, userArgs.PageSize, convert =>
            {
                return convert.Select(s => new UserDto
                {
                    Id = s.Id,
                    Account = s.Account,
                    FullName = $"{s.Surname} {s.Name} {s.Middlename}",
                    ShortName = $"{s.Surname} {s.Name.Substring(0, 1)}.{s.Middlename.Substring(0, 1)}.",
                    Email = s.Email,
                    Phone = s.Phone,
                    TypeUser = s.TypeUser == null ? string.Empty : s.TypeUser.Name
                });
            });
            return result;
        }

        /// <summary>
        /// Получить список ролей пользователя
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetRoleUsers(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return Enumerable.Empty<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        /// <summary>
        /// Добавить роль для пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCrmDb> AddToRoleByUser(string userName, string roleName)
        {
            var result = new ResultCrmDb();
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                result.Succeeded = false;
                result.AddError("crm002", "Пользователя с таким аакаунтом не существует");
                return result;
            }

            var resultIdentity = await _userManager.AddToRoleAsync(user, roleName);
            if (resultIdentity.Succeeded) return result;
            result.Succeeded = false;
            foreach (var error in resultIdentity.Errors)
            {
                result.AddError(error.Code, error.Description);
            }

            var userCrm = await _crmDbContext.User.FirstOrDefaultAsync(f => f.Account == userName);
            var roleCrm = await _crmDbContext.UserRole.FirstOrDefaultAsync(f => f.Sysname == roleName);
            var link = await _crmDbContext.UserLinkRole.FirstOrDefaultAsync(
                f => f.User == userCrm && f.UserRole == roleCrm);
            if (link != null)
                return result;
            link = new UserLinkRole() { User = userCrm, UserRole = roleCrm };
            _crmDbContext.Add(link);
            await _crmDbContext.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// Убрать роль у пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="roleName">Название роли</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> RemoveFromRoleByUser(string userName, string roleName)
        {
            var result = new ResultCrmDb();
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                result.Succeeded = false;
                result.AddError("crm002", "Пользователя с таким аакаунтом не существует");
                return result;
            }

            var resultIdentity = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (resultIdentity.Succeeded) return result;
            result.Succeeded = false;
            foreach (var error in resultIdentity.Errors)
            {
                result.AddError(error.Code, error.Description);
            }

            var link = await _crmDbContext.UserLinkRole.FirstOrDefaultAsync(
                f => f.User.Account == userName && f.UserRole.Sysname == roleName);
            if (link == null)
                return result;
            _crmDbContext.Remove(link);
            await _crmDbContext.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// Получить информацию о переданном пользователе
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns>Модель пользователя</returns>
        public async Task<UserDto> GetUser(string userName)
        {
            await _crmDbContext.User.LoadAsync();
            var user = await _crmDbContext.User.FirstOrDefaultAsync(f => f.Account == userName);
            if (user == null)
                throw new UserMessageException( $"Пользователя {userName} нет в БД");
            var userDto = new UserDto
            {
                Id = user.Id,
                Account = user.Account,
                FullName = $"{user.Surname} {user.Name} {user.Middlename}",
                ShortName = $"{user.Surname} {user.Name.Substring(0, 1)}.{user.Middlename.Substring(0, 1)}.",
                Email = user.Email,
                Phone = user.Phone,
                TypeUser = user.TypeUser == null ? string.Empty : user.TypeUser.Name
            };
            return userDto;
        }

        /// <summary>
        /// Получить список всех типов пользователей
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TypeUserDto> GetTypeUsers()
        {
            return  _crmDbContext.TypeUser.Select(s => new TypeUserDto
            {
                Id = s.Id,
                Name = s.Name
            });
        }
        /// <summary>
        /// Добавление новой роли
        /// </summary>
        /// <param name="roleName">имя роли</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> AddRoleAsync(string roleName)
        {
            var resultCrmDb = new ResultCrmDb();
            var result = await _roleManager.CreateAsync(new ApplicationRole { Name = roleName});

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    resultCrmDb.AddError(error.Code, error.Description);
                }
                return resultCrmDb;
            }

            var userRole = await _crmDbContext.UserRole.FirstOrDefaultAsync(f => f.Sysname == roleName);
            if (userRole != null)
                return resultCrmDb;
            userRole = new UserRole();
            userRole.Sysname = roleName;
            userRole.Name = roleName;
            _crmDbContext.Add(userRole);
            await _crmDbContext.SaveChangesAsync();
            return resultCrmDb;
        }

        /// <summary>
        /// Удаление роли 
        /// </summary>
        /// <param name="roleName">Имя роли</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> RemoveRole(string roleName)
        {
            var resultCrmDb = new ResultCrmDb();
            var role = await _roleManager.FindByNameAsync(roleName);
            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    resultCrmDb.AddError(error.Code, error.Description);
                }
                return resultCrmDb;
            }

            var userRole = await _crmDbContext.UserRole.FirstOrDefaultAsync(f => f.Sysname == roleName);
            if (userRole == null)
                return resultCrmDb;
            _crmDbContext.Remove(userRole);
            await _crmDbContext.SaveChangesAsync();

            return resultCrmDb;
        }
    }


}
