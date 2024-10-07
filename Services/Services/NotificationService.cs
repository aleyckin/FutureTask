using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;

        public NotificationService(IConfiguration configuration) { _configuration = configuration; }

        public async Task SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("FutureTask", _configuration["EmailSettings:FromEmail"]));
            emailMessage.To.Add(new MailboxAddress("", toEmail));

            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(
                        _configuration["EmailSettings:SmtpServer"],
                        int.Parse(_configuration["EmailSettings:SmtpPort"]),
                        true,
                        cancellationToken
                    );

                    await client.AuthenticateAsync(
                        _configuration["EmailSettings:FromEmail"],
                        _configuration["EmailSettings:Password"],
                        cancellationToken
                    );

                    await client.SendAsync(emailMessage, cancellationToken);
                }
                finally
                {
                    await client.DisconnectAsync(true, cancellationToken);
                }
            }
        }
    }
}
