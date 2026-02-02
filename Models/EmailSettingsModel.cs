namespace Autentication.Models
{
    public class EmailSettingsModel
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string senderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
    }
}
