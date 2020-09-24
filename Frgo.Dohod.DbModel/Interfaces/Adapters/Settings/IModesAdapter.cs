using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelDto.Settings;
using Frgo.Dohod.DbModel.Models.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frgo.Dohod.DbModel.Interfaces.Adapters.Settings
{
    public interface IModesAdapter
    {
        /// <summary>
        /// Получить дерево элементов для выбора доступных форм по роли
        /// </summary>
        /// <param name="idRole"></param>
        /// <returns></returns>
        Task<ModeAccessDto> GetModesTreeAsync(int idRole);
        /// <summary>
        /// Настройка доступа к режимам по роли
        /// </summary>
        /// <param name="idRole"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        Task<ResultCrmDb> EditAccessAsync(int idRole, int[] selected);
    }
}
