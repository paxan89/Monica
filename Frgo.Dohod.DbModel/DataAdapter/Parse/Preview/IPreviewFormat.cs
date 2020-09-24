using System.Threading.Tasks;
using Frgo.Dohod.DbModel.ModelCrm.Core;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.Preview
{
    /// <summary>
    /// Главный сервис для парса файлов
    /// </summary>
    public interface IPreviewFormat
    {
        Task<object> PreviewFormat(object doc, ImportParam importParam);
    }
}
