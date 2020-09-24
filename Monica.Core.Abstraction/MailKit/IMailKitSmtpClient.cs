using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Monica.Core.Abstraction.MailKit
{
	/// <summary>
	/// Интерфейс для получения клиента SMTP
	/// </summary>
    public interface IMailKitSmtpClient
    {
        /// <summary>
        /// smtp client
        /// </summary>
        Task<SmtpClient> SmtpClient();

    }
}
