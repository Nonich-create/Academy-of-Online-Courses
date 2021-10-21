using MailKit.Net.Smtp;
using MimeKit;
using Students.BLL.DataAccess;
using Students.BLL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students.BLL.EmailSend
{
    public class EmailSenderService : IEmailSenderService
    {

        public EmailSenderService()
        {
        }

        private static async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "makita.ayasaki@yandex.by"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.yandex.ru", 465, true);
            await client.AuthenticateAsync("makita.ayasaki@yandex.by", "w2H-Vei-Z6d-t5U");
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }

        public async Task SendAcceptanceConfirmation(string email, string text)
        {
            await SendEmailAsync(email, "Зачисления на курс", text);
        }
 
    }
}
 
