using Frgo.Dohod.DbModel.Interfaces.Adapters.Settings;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Profile;
using Frgo.Dohod.DbModel.ModelDto;
using Frgo.Dohod.DbModel.ModelDto.Users;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Registration;
using Monica.Core.ModelParametrs.ModelsArgs;
using Monica.CrmDbModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using MonicaUser = Monica.Core.DbModel.ModelCrm.Profile.User;

namespace Frgo.Dohod.DbModel.DataAdapter.Settings
{
    /// <summary>
    /// Адаптер для работы с ролями пользователей
    /// </summary>
    public class UsersAdapter : IUsersAdapter
    {
        private IRegistrationUserAdapter _registration;
        private DohodDbContext _crmDbContext;
        public UsersAdapter(DohodDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int idRole)
        {
            var result = new List<UserDto>();
            try
            {
                var links = await _crmDbContext.userlinkrole.Where(r => r.UserRoleId == idRole).ToListAsync();
                if (links.Count() == 0)
                    throw new Exception($"В данной роли отсутствуют пользователи.");//{levelOrg.Caption}
                links.ForEach(r => result.Add(_crmDbContext.user.FirstOrDefault(u => u.Id == r.UserId)));
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var result = new List<UserDto>();
            try
            {
                var users = await _crmDbContext.user.ToListAsync();
                foreach (var u in users)
                {
                    result.Add(u);
                }
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
        public async Task<ResultCrmDb> RegisterUserAsync(RegistrationUserArgs args)
        {
            var result = new ResultCrmDb();
            var errors = 0;
            List<string> textErrors = new List<string>();
            try
            {
                if (string.IsNullOrWhiteSpace(args.Account))
                {
                    textErrors.Add("аккаунт пользователя не указан");
                    errors++;
                }
                if (string.IsNullOrWhiteSpace(args.Name))
                {
                    textErrors.Add("имя пользователя не указано");
                    errors++;
                }
                if (errors > 0)
                    throw new Exception();
                var countUser = await _crmDbContext.User.CountAsync(c => c.Account.ToLower() == args.Account.ToLower());
                if (countUser > 0)
                {
                    textErrors.Add("пользователь с таким логином уже существует");
                    errors++;
                }
                var user = new User();
                user.Account = args.Account; 
                user.Email = args.Email;
                user.Phone = args.Phone;
                user.Name = args.Name;
                user.Surname = args.Surname;
                user.Middlename = args.Middlename;
                await _crmDbContext.user.AddAsync(user);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var err = "";
                
                if (errors > 0)
                    err = string.Join(",\n", textErrors.ToArray());
                else
                    err = e.Message;
                var patternError = $"Ошибка регистрации пользователя:\n {err}.";
                result.AddError("",$"{patternError}" );

            }
            return result;
        }
        public async Task<ResultCrmDb> RemoveUsersAsync(IEnumerable<string> accUsers)
        {
            var result = new ResultCrmDb();
            try
            {
                List</*MonicaUser*/Monica.Core.DbModel.ModelCrm.Profile.User> users = new List<Monica.Core.DbModel.ModelCrm.Profile.User/*MonicaUser*/>();
                foreach(var acc in accUsers)
                {
                    var user = await _crmDbContext.User.FirstOrDefaultAsync(u =>u.Account.ToLower() == acc.ToLower());
                    _crmDbContext.userlinkrole.RemoveRange(_crmDbContext.userlinkrole.Where((ur) => ur.UserId == user.Id));
                    users.Add(user);
                }
                await _crmDbContext.SaveChangesAsync();
                _crmDbContext.User.RemoveRange(users);
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
