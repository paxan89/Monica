using System.Data;
using SqlKata.Compilers;

namespace Monica.Core.Abstraction.ReportEngine
{
    /// <summary>
    /// Класс создателя соединений к БД
    /// </summary>
    public interface IConnectorManager
    {
        /// <summary>
        /// Получить подключение к БД
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();
        /// <summary>
        /// Фабрика для комилятора 
        /// </summary>
        Compiler Compiler { get; }
    }
}
