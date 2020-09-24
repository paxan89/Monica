using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelDto.Report;
using Newtonsoft.Json.Linq;

namespace Monica.Core.Abstraction.ReportEngine
{
    public interface IButtonCreater
    {
        /// <summary>
        /// Получить список доступных действий для данного режима
        /// </summary>
        /// <param name="buttons">Список описанных кнопок</param>
        /// <returns></returns>
        Task<JArray> GetButtonJson(IEnumerable<ButtonAccessDto> buttons);
    }
}
