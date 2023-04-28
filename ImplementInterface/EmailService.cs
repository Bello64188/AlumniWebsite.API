using AlumniWebsite.API.Interface;
using AlumniWebsite.API.Model;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ImplementInterface
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettings> emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            this.emailSettings = emailSettings;
        }
        public async Task SendEmail(EmailRequest emailRequest, CancellationToken ct)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Value.Mail);
            email.From.Add(MailboxAddress.Parse(emailSettings.Value.Mail));
            email.To.Add(MailboxAddress.Parse(emailRequest.ToEmail));
            email.Subject = emailRequest.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = emailRequest.Body;
            email.Body = builder.ToMessageBody();

            var client = new SmtpClient();
            client.Connect(emailSettings.Value.Host, emailSettings.Value.Port, SecureSocketOptions.StartTls);
            client.Authenticate(emailSettings.Value.Username, emailSettings.Value.Password);
            await client.SendAsync(email, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}
