using System.Threading.Tasks;
using Frgo.Dohod.DbModel.ModelCrm.Core;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.File
{
    /// <summary>
    /// Главный сервис для парса файлов
    /// </summary>
    public interface IParseFile
    {
        Task<ResultCrmDb> ParseFile();
        byte[] FileData { get; set; }
        ImportParam ImportParam { get; set; }

    }
}