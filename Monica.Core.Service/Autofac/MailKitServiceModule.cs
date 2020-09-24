using Autofac;
using Monica.Core.Abstraction.MailKit;
using Monica.Core.Attributes;
using Monica.Core.Service.MailKit;

namespace Monica.Core.Service.Autofac
{
    [CommonModule]
    public class MailKitServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmailConfigService>().As<IEmailConfigService>();
            builder.RegisterType<MailKitSmtpClient>().As<IMailKitSmtpClient>();
            builder.RegisterType<EmailService>().As<IEmailService>();

        }
    }
}
