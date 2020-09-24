using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MimeKit.Text;
using Monica.Core.Abstraction.MailKit;

namespace Monica.Core.Service.MailKit
{
	/// <summary>
	/// Служба отправки писем
	/// </summary>
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Получение настроенного клиента
        /// </summary>
        private IEmailConfigService _emailConfigService;
        private IMailKitSmtpClient _mailKitSmtpClient;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="emailConfigService"></param>
        /// <param name="mailKitSmtpClient"></param>
        public EmailService(IEmailConfigService emailConfigService, IMailKitSmtpClient mailKitSmtpClient)
        {
            _emailConfigService = emailConfigService;
            _mailKitSmtpClient = mailKitSmtpClient;
        }


		/// <summary>
		/// Отправка письма
		/// </summary>
		/// <param name="mailTo">Кому e-mail, разделен ","</param>
		/// <param name="mailCc">Копия e-mail, разделен ","</param>
		/// <param name="mailBcc">Скрытая копия e-mail, разделен ","</param>
		/// <param name="subject">Тема</param>
		/// <param name="message">Тело</param>
		/// <param name="encoding">Кодировка</param>
		/// <param name="isHtml">Это HTML</param>
		public async Task Send(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, bool isHtml = false)
        {
            await SendEmail(mailTo, mailCc, mailBcc, subject, message, encoding, isHtml);
        }


		/// <summary>
		/// Отправка письма, асинхронно
		/// </summary>
		/// <param name="mailTo">Кому e-mail, разделен ","</param>
		/// <param name="mailCc">Копия e-mail, разделен ","</param>
		/// <param name="mailBcc">Скрытая копия e-mail, разделен ","</param>
		/// <param name="subject">Тема</param>
		/// <param name="message">Тело</param>
		/// <param name="encoding">Кодировка</param>
		/// <param name="isHtml">Это HTML</param>
		public async Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, bool isHtml = false)
        {
            await SendEmail(mailTo, mailCc, mailBcc, subject, message, encoding, isHtml);
        }

		/// <summary>
		/// Непосредственно отправка письма
		/// </summary>
		/// <param name="mailTo">Кому e-mail, разделен ","</param>
		/// <param name="mailCc">Копия e-mail, разделен ","</param>
		/// <param name="mailBcc">Скрытая копия e-mail, разделен ","</param>
		/// <param name="subject">Тема</param>
		/// <param name="message">Тело</param>
		/// <param name="encoding">Кодировка</param>
		/// <param name="isHtml">Это HTML</param>
		private async Task SendEmail(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, bool isHtml)
        {
            var modelSettings = await _emailConfigService.GetConfig();
            var to = new string[0];
            var cc = new string[0];
            var bcc = new string[0];
            if (!string.IsNullOrEmpty(mailTo))
	            to = mailTo.Split(',').Select(x => x.Trim()).ToArray();
            if (!string.IsNullOrEmpty(mailCc))
                cc = mailCc.Split(',').Select(x => x.Trim()).ToArray();
            if (!string.IsNullOrEmpty(mailBcc))
                bcc = mailBcc.Split(',').Select(x => x.Trim()).ToArray();

			// HACK добавить проверку корректности E-mail ?
            var mimeMessage = new MimeMessage();

            // от кого письмо имя и обратный адрес
            mimeMessage.From.Add(new MailboxAddress(modelSettings.SmtpEmailDisplay, modelSettings.SmtpEmail));

            // кому
            foreach (string tos in to)
            {
                mimeMessage.To.Add(MailboxAddress.Parse(tos));
            }

            // копия
            foreach (string ccs in cc)
            {
                mimeMessage.Cc.Add(MailboxAddress.Parse(ccs));
            }

            // скрытая копия
            foreach (string bccs in bcc)
            {
                mimeMessage.Bcc.Add(MailboxAddress.Parse(bccs));
            }

            // тема письма
            mimeMessage.Subject = subject;

            // тело письма
            TextPart body;

            body = isHtml ? new TextPart(TextFormat.Html) : new TextPart(TextFormat.Text);
            
            // кодировка
            body.SetText(encoding, message);

            // тело письма
            mimeMessage.Body = body;

            using (var client = await _mailKitSmtpClient.SmtpClient())
            {
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
			}
        }
    }
}
