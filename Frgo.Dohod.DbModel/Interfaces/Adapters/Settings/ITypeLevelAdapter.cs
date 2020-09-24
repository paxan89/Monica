using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.ModelDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frgo.Dohod.DbModel.Interfaces.Adapters.Settings
{
    public interface ITypeLevelAdapter
    {
        /// <summary>
        /// Получить список типов уровней орг (для отображения вместо цифр строки на web-интерфейсе)
        /// </summary>
        /// <returns></returns>
        Task<List<TypeLevelDto>> GetListTypeLevels();
        /// <summary>
        /// Получить строковое отображения для уровня орг по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetTypeLevelById(int id);
        /// <summary>
        /// Получить уровень орг по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HelperEnum.TypeLevels GetTypeLevelEnumById(int id);
    }
}
