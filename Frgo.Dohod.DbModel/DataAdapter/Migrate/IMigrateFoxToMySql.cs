using System.Threading.Tasks;
using Frgo.Dohod.DbModel.ModelCrm.Core;

namespace Frgo.Dohod.DbModel.DataAdapter.Migrate
{
    /// <summary>
    /// Копировать с FOX
    /// </summary>
    public interface IMigrateFoxToMySql
    {
        /// <summary>
        /// Копировать с FOX
        /// </summary>
        /// <returns></returns>
        Task<ResultCrmDb> MigrateFromFox(string dirFoxDb, System.Text.Encoding encoding);
    }
}
