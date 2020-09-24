namespace Monica.MailKit.EMailConfigService
{
    public class SendEMailConfiguration
    {
        public int JobSendEMailInterval { get; set; }
        public string SmtpEmail { get; set; }
        public string SmtpEmailDisplay { get; set; }
        public string SmtpLogin { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpEnableSSL { get; set; }
        public int NumberItemsSendEMailGroup { get; set; }

        public bool IsValid()
        {
            if (JobSendEMailInterval.Equals(default(int)))
            {
                JobSendEMailInterval = 30; 
            }

            return !string.IsNullOrEmpty(SmtpEmail) && 
                   !string.IsNullOrEmpty(SmtpEmailDisplay) && 
                   !string.IsNullOrEmpty(SmtpLogin) && 
                   !string.IsNullOrEmpty(SmtpPassword) && 
                   !string.IsNullOrEmpty(SmtpServer) && 
                   SmtpPort != default(int) && 
                   NumberItemsSendEMailGroup != default(int);
        }
    }
}
