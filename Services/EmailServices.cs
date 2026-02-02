using System.Net.Mail;

using Autentication.Models;

namespace Autentication.Services;

public class EmailServices : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailServices(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var from = _configuration["EmailSettings:Username"];
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var port = int.Parse(_configuration["EmailSettings:Port"] ?? "25");
        var password = _configuration["EmailSettings:Password"];
        var displayName = _configuration["EmailSettings:DisplayName"];

        var message = new MailMessage(from!, toEmail, subject, body);
        message.IsBodyHtml = true;

        using var client = new SmtpClient(smtpServer, port)
        {
            Credentials = new System.Net.NetworkCredential(from, password),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }
}