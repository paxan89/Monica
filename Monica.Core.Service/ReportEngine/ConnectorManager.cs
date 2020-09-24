using System.Data;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DataBaseUtils;
using Monica.Core.Utils;
using MySql.Data.MySqlClient;
using SqlKata.Compilers;

namespace Monica.Core.Service.ReportEngine
{
    /// <summary>
    /// Получение соединения с БД
    /// </summary>
    public class ConnectorManager : IConnectorManager
    {
        private readonly IDataBaseMain _dataBaseMain;

        public ConnectorManager(IDataBaseMain dataBaseMain)
        {
            _dataBaseMain = dataBaseMain;
        }

        /// <summary>
        /// Получить соединение с БД
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            if (_dataBaseMain.TypeDataBase == DataBaseName.MySql)
                return new MySqlConnection(_dataBaseMain.ConntectionString);
            return new MySqlConnection(_dataBaseMain.ConntectionString);
        }

        public Compiler Compiler
        {
            get
            {
                switch (_dataBaseMain.TypeDataBase)
                {
                    case DataBaseName.MySql: return new MySqlCompiler();
                }
                return new MySqlCompiler();
            }
        }

    }
}
