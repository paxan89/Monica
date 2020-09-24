using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.Interfaces.Adapters.Settings;
using Frgo.Dohod.DbModel.ModelDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frgo.Dohod.DbModel.DataAdapter.Settings
{
    public class TypeLevelAdapter : ITypeLevelAdapter
    {
        /// <summary>
        /// Словарь для сопоставления перечисления уровня орг cо строкой
        /// </summary>
        public static Dictionary<HelperEnum.TypeLevels, string> TypeLevelsDic = new Dictionary<HelperEnum.TypeLevels, string>()
        {
            {HelperEnum.TypeLevels.Ufk ,"Управление Федерального казначейства" },
            {HelperEnum.TypeLevels.Mf,"Минестерство финансов" },
            {HelperEnum.TypeLevels.Fo,"Финансовый орган" },
            {HelperEnum.TypeLevels.GAdm,"Главный Администратор доходов" },
            {HelperEnum.TypeLevels.Adm,"Администратор доходов" } };
        public TypeLevelAdapter() { }
        public async Task<List<TypeLevelDto>> GetListTypeLevels()
        {
            var result = new List<TypeLevelDto>();
            foreach (var level in TypeLevelsDic)
            {
                result.Add(new TypeLevelDto() { Id = (int)level.Key, Caption = level.Value });
            }
            return result;
        }
        public string GetTypeLevelById(int id)
        {
            return TypeLevelsDic[(HelperEnum.TypeLevels)id];
        }
        public HelperEnum.TypeLevels GetTypeLevelEnumById(int id)
        {
            return (HelperEnum.TypeLevels)id;
        }
    }

}
