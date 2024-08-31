using Demo.DAL.Models;
using Demo.PL.ConfigSetting;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;

namespace Demo.PL.Helpers
{
    public class NewEmailSettings : INewEmailSettings
    {
 
        public NewEmailSettings()
        {
            
        }

        private readonly EmailConfigSetting _options;
        public NewEmailSettings(IOptions<EmailConfigSetting> options)
        {
            _options = options.Value;
        }
        public void SendEmail(NewEmail email)
        {
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_options.Email),
                Subject = email.Subject,
            };


            var builder = new BodyBuilder();
            builder.TextBody = email.Body;

            mail.Body = builder.ToMessageBody();

            mail.From.Add(new MailboxAddress(_options.DisplayName, _options.Email));

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            smtp.Connect(_options.Host, _options.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.Email, _options.Password);
            smtp.Send(mail);
            smtp.Disconnect(true);
        }
    }
}
