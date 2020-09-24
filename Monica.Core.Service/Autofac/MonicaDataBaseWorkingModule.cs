using Autofac;
using Monica.Core.Abstraction.Profile;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.Attributes;
using Monica.Core.Service.Profile;
using Monica.Core.Service.ReportEngine;

namespace Monica.CrmDbModel.Autofac
{
    /// <summary>
    /// Модуль IoC контейнера
    /// </summary>
    [CommonModule]
    public class MonicaDataBaseWorkingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Загрузить работу с профилями пользователей
            builder.RegisterType<ManagerProfile>().As<IManagerProfile>();
            // Загрузить работу с доступом
            builder.RegisterType<AccessManager>().As<IAccessManager>();
            // Загрузить работу с данными
            builder.RegisterType<ReportData>().As<IReportData>();
            //Менеджер управления режимами
            builder.RegisterType<ReportManager>().As<IReportManager>();
            // Загрузить генератор данных под структуру MySql
            builder.RegisterType<GenerateFieldMySql>().Named<IGenerateField>(nameof(GenerateFieldMySql));
            //Менеджер получение объектов БД
            builder.RegisterType<ConnectorManager>().As<IConnectorManager>();
            //Регистрация сервиса формированрия данных для конструктора по умолчанию
            builder.RegisterType<ReportEngineDefaultData>().Named<IReportEngineData>(nameof(ReportEngineDefaultData));
            //Регистрация генератора колонок по умолчанию
            builder.RegisterType<ColumnCreater>().As<IColumnCreater>();
            //Регистрация генератора кнопок по умолчанию
            builder.RegisterType<ButtonCreater>().As<IButtonCreater>();
        }
    }
}
