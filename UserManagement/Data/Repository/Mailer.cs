using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;
using UserManagement.Data.Interface;
using UserManagement.Models;
using System.Net;
using System.Net.Mail;
using System;

namespace UserManagement.Data.Repository
{
    public class Mailer : IMailer
    {
        private readonly SmtpSettings _stmpSettings;
        public Mailer(IOptions<SmtpSettings> stmpSettings)
        {
            this._stmpSettings = stmpSettings.Value; 
        }

        public async Task SendEmail(string content, string ToEmail, string subject, string Title)
        {
            try
            {

                var smtpClient = new SmtpClient()
                {
                    Host = _stmpSettings.Server,
                    Port = _stmpSettings.Port,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_stmpSettings.UserName, _stmpSettings.Password)
                };
                var message = new MailMessage(_stmpSettings.UserName, ToEmail);
                message.Subject = subject;
                message.Body = Title + content;
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

    }
}
