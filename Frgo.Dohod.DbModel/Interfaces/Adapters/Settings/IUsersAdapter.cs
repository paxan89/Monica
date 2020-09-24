using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelDto;
using Frgo.Dohod.DbModel.ModelDto.Users;
using Monica.Core.ModelParametrs.ModelsArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Frgo.Dohod.DbModel.Interfaces.Adapters.Settings
{
    public interface IUsersAdapter
    {
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int idRole);
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<ResultCrmDb> RegisterUserAsync(RegistrationUserArgs args);
        Task<ResultCrmDb> RemoveUsersAsync(IEnumerable<string> accUsers);
        //Task<ResultCrmDb> EditUserRoleAsync(int sysIdRole, string newName);

    }
}
