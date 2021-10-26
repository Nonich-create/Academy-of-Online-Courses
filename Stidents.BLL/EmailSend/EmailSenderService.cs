using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Students.BLL.Interface;
using Students.DAL.Models;
using System;
using System.Threading.Tasks;

namespace Students.BLL.EmailSend
{
    public class EmailSenderService : IEmailSenderService
    {

        public EmailSenderService()
        {
        }

        private async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            var builder = new BodyBuilder
            {
                TextBody = subject,
                HtmlBody = message
            };
            emailMessage.From.Add(new MailboxAddress("Администрация сайта Академия онлайн курсов", "makita.ayasaki@yandex.by"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = builder.ToMessageBody();
            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.yandex.ru", 465, true);
            await client.AuthenticateAsync("makita.ayasaki@yandex.by", "w2H-Vei-Z6d-t5U");
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
            
        }

        public async Task SendAcceptanceConfirmation(string email, Group group, Student student)
        {
            string HtmlForm = $"<p>Вас зачислели на курс {group.Course.Name} номер вашей группы {group.NumberGroup}.<br>" +
                $"<p>Менеджер группы {group.Manager.Surname} {group.Manager.Name} {group.Manager.MiddleName}.<br>" +
                $"<p>Преподователь группы {group.Teacher.Surname} {group.Teacher.Name} {group.Teacher.MiddleName}.<br>" +
                $"<p>Дата старта группы: {group.DateStart:D}";
            await SendEmailAsync(email, "Зачисления на курс", HtmlForm);
        }
 
    }
}
