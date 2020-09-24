using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Data;
using Frgo.Dohod.DbModel.ModelCrm.ImportPreviw;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frgo.Dohod.DbModel.Manager.Parse
{
    /// <summary>
    /// Главный сервис для парса файлов
    /// </summary>
    public interface IParserFormat
    {
        Task<object> ParserFormat(object doc, ImportParam importParam);
        public ImpPreviewData PreviewData { get; }
        abstract Task<bool> AfterDataWrited(List<t_dohod> dohods, ImportParam importParam);

    }
}
