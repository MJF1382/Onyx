using Onyx.Classes;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Onyx.Services
{
    public class MailSender : IMailSender
    {
        private readonly IOptions<MailSettings> _mailSettingsOptions;

        public MailSender(IOptions<MailSettings> mailSettingsOptions)
        {
            _mailSettingsOptions = mailSettingsOptions;
        }

        public async Task SendAsync(Email email)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(_mailSettingsOptions.Value.SenderMail);
            message.To.Add(email.To);
            message.Subject = email.Subject;
            message.Body = email.Body;
            message.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient(_mailSettingsOptions.Value.SmtpServer);
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(_mailSettingsOptions.Value.SenderMail, "Fathi2831Fathi");
            await smtpClient.SendMailAsync(message);
        }
    }
}
