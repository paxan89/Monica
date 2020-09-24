using Autofac;
using Monica.Core.Attributes;

namespace Frgo.Dohod.WebApi.Autofac
{
    /// <summary>
    /// Модуль регистрации компонентов в автофак
    /// </summary>
    [CommonModule]

    public class DohodWebApiModule : Module
    {
        /// <summary>
        /// Загрузка компонентов модуля
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {

        }
    }
}
