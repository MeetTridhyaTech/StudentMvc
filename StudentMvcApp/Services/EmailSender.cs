using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StudentMvcApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void send(string to, string subject, string body)
        {
            try
            { 
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpClient = new SmtpClient(emailSettings["Host"])
                {
                    Port = int.Parse(emailSettings["Port"]),
                    Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
                    EnableSsl = bool.Parse(emailSettings["EnableSsl"]),
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings["FromEmail"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(to);
                smtpClient.Send(mailMessage);

                _logger.LogInformation("Email sent successfully to {Email}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending email to {Email}", to);
            }
        }
    }
}
