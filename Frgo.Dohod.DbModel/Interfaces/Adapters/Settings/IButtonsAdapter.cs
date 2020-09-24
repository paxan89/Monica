using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelDto.Settings;
using Frgo.Dohod.DbModel.Models.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frgo.Dohod.DbModel.Interfaces.Adapters.Settings
{
    public interface IBtnsAdapter
    {
        /// <summary>
        /// Получить дерево элементов для выбора доступных кнопок по роли
        /// </summary>
        /// <param name="idRole"></param>
        /// <returns></returns>
        Task<ButtonAccessDto> GetButtonsTreeAsync(int idRole);
        /// <summary>
        /// Настройка доступа к кнопкам по роли
        /// </summary>
        /// <param name="idRole"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        Task<ResultCrmDb> EditAccessAsync(int idRole, int[] selected);
    }
}
