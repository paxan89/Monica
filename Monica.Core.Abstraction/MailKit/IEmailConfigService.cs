using System.Threading.Tasks;
using Monica.MailKit.EMailConfigService;

namespace Monica.Core.Abstraction.MailKit
{
    public interface IEmailConfigService
    {
        /// <summary>
        /// Получение настроек для SMTP и других
        /// </summary>
        /// <returns></returns>
        Task<SendEMailConfiguration> GetConfig();
    }
}
