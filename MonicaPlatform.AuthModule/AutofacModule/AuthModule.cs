using Autofac;
using Monica.Core.Attributes;
using MonicaPlatform.AuthModule.Middleware;

namespace MonicaPlatform.AuthModule.AutofacModule
{
    [CommonModule]
    public class AuthModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthUserIs4>().Named<IAuthUserEngine>("IS4");
        }
    }
}
