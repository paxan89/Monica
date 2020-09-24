using Autofac;
using Monica.Core.Abstraction.Registration;
using Monica.Core.Attributes;
using Monica.Core.Service.Registration;

namespace Monica.Core.Service.Autofac
{
    /// <summary>
    /// Модуль автофака
    /// </summary>
    [CommonModule]
    public class MonicaAdapterModule : Module
    {
        /// <summary>
        /// загрузить записимости
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RegistrationUserAdapter>().As<IRegistrationUserAdapter>();
        }
    }
}
