using AB460Proniya.Interfaces;
using System.Net.Mail;
using System.Net;

namespace AB460Proniya.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMailAsync(string emailTo, string subject, string body, bool isHTML = false)
        {
            SmtpClient smtp = new SmtpClient(_config["Email:Host"], Convert
                .ToInt32(_config["Email:Port"]));

            smtp.EnableSsl = true;

            smtp.Credentials = new NetworkCredential(_config["Email:LoginEmail"], _config["Email:Password"]);

            MailAddress from = new MailAddress(_config["Email:LoginEmail"], "Pronia Administration");

            MailAddress to = new MailAddress(emailTo);

            MailMessage message = new MailMessage(from, to);

            message.Subject = subject;

            message.Body = body;


            message.IsBodyHtml = isHTML;

            await smtp.SendMailAsync(message);
        }
    }
}
