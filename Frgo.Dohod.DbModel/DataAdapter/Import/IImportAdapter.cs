using System.Threading.Tasks;
using Frgo.Dohod.DbModel.ModelCrm.Core;

namespace Frgo.Dohod.DbModel.DataAdapter.Import
{
    /// <summary>
    /// Основной сервис загрузки распарсеных файлов
    /// </summary>
    public interface IImportAdapter
    {
        /// <summary>
        /// Импорт файла
        /// </summary>
        /// <returns></returns>
        Task<ResultCrmDb> ImportFile(byte[] fileData, ImportParam importParam);
        Task<ResultCrmDb> PreviewFile(byte[] fileData, ImportParam importParam);
        Task<ResultCrmDb> LoadFkDocxFormat(byte[] fileData, ImportParam importParam);
   }
}
