using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Monica.Core.Abstraction.MailKit;
using Monica.MailKit.EMailConfigService;

namespace Monica.Core.Service.MailKit
{
    public class EmailConfigService : IEmailConfigService
    {
        private IConfiguration _configuration;
        /// <summary>
        /// Конструктор
        /// </summary>
        public EmailConfigService()
        {
            var confFileName = Path.Combine(
                Path.GetDirectoryName(GetType().Assembly.Location),
                $"Monica.MailKit.dll.config");
            var build = new ConfigurationBuilder().AddXmlFile(confFileName);
            _configuration = build.Build();

        }
      
        /// <summary>
        /// Получить конфигурацию из конфигурационного файла
        /// </summary>
        /// <returns></returns>
        public async Task<SendEMailConfiguration> GetConfig()
        {
            return new SendEMailConfiguration
            {
                SmtpEmail = _configuration["SmtpEmail"],
                SmtpEmailDisplay = _configuration["SmtpEmailDisplay"],
                SmtpEnableSSL = Convert.ToBoolean(_configuration["SmtpEnableSSL"]),
                SmtpLogin = _configuration["SmtpLogin"],
                SmtpPassword = _configuration["SmtpPassword"],
                SmtpPort = Convert.ToInt32(_configuration["SmtpPort"]),
                SmtpServer = _configuration["SmtpServer"]
            };  
        }
    }
}
