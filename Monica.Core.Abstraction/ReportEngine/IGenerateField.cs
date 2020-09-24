using System.Threading.Tasks;

namespace Monica.Core.Abstraction.ReportEngine
{
    /// <summary>
    /// Генерация полей для режимов на основании схемы БД 
    /// </summary>
    public interface IGenerateField
    {
        /// <summary>
        /// Генерация полей для режима на основании схемы БД
        /// </summary>
        /// <param name="formModelId"></param>
        /// <returns></returns>
        Task GenerateField(int formModelId);
        /// <summary>
        /// Генерировать кнопки для управления режимом
        /// </summary>
        /// <param name="formModelId">Ссылка на модель </param>
        /// <returns></returns>
        Task GenerateDefaultBtn(int formModelId);
    }
}
