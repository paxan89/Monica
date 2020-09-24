using Autofac;
using Monica.Core.Abstraction.Authorize;
using Monica.Core.Attributes;
using Monica.Core.Service.Authorize;

namespace Monica.Core.Service.Autofac
{
    /// <summary>
    /// Модуль регистрации компонентов в автофак
    /// </summary>
    [CommonModule]

    public class MonicaCrmModule : Module
    {
        /// <summary>
        /// Загрузка компонентов модуля
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MonicaAuthorizeDataAdapter>().As<IMonicaAuthorizeDataAdapter>();
        }
    }
}
