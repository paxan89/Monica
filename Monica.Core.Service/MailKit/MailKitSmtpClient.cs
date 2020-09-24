using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Monica.Core.Abstraction.MailKit;

namespace Monica.Core.Service.MailKit
{
    /// <summary>
    /// Клиент для SMTP
    /// </summary>
    public class MailKitSmtpClient : IMailKitSmtpClient
    {
        /// <summary>
        /// Настройки
        /// </summary>
        private readonly IEmailConfigService _emailConfigService;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="emailConfigService"></param>
        public MailKitSmtpClient(IEmailConfigService emailConfigService)
        {
            _emailConfigService = emailConfigService;
        }

        #region Smtp клиент

        public async Task<SmtpClient> SmtpClient() => await InitSmtpClient();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<SmtpClient> InitSmtpClient()
        {
            var model = await _emailConfigService.GetConfig();
            var smtpClient = new SmtpClient { ServerCertificateValidationCallback = (s, c, h, e) => true };

            smtpClient.Connect(model.SmtpServer, model.SmtpPort,
                !model.SmtpEnableSSL ? SecureSocketOptions.None : SecureSocketOptions.Auto);

            // since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");

            // Если 
            if (!string.IsNullOrEmpty(model.SmtpLogin) && !string.IsNullOrEmpty(model.SmtpPassword))
                smtpClient.Authenticate(model.SmtpLogin, model.SmtpPassword);

            return smtpClient;
        }

        #endregion


    }
}
