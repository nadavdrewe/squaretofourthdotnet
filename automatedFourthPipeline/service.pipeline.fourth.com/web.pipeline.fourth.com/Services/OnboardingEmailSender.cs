using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace web.pipeline.fourth.com.Services
{
    public class OnboardingEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OnboardingEmailSender> _logger;
        public OnboardingEmailSender(IConfiguration configuration, ILogger<OnboardingEmailSender> logger) { _configuration = configuration; _logger = logger; }
        public async Task<bool> SendAsync(string email, string clientName, string password)
        {
            var host = _configuration["Email:Smtp:Host"];
            var from = _configuration["Email:Smtp:FromAddress"];
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from)) return false;
            try
            {
                using var smtp = new SmtpClient(host, _configuration.GetValue<int>("Email:Smtp:Port", 587)) { EnableSsl = _configuration.GetValue("Email:Smtp:EnableSsl", true) };
                var username = _configuration["Email:Smtp:Username"];
                var secret = _configuration["Email:Smtp:Password"];
                if (!string.IsNullOrWhiteSpace(username)) smtp.Credentials = new NetworkCredential(username, secret);
                using var message = new MailMessage(from, email) { Subject = "Your Square to Fourth integration login", Body = $"Your {clientName} integration is ready.\n\nLogin: {email}\nTemporary password: {password}\n\nSign in at {_configuration["PublicSite:BaseUrl"] ?? "https://squaretofourth.store"} and connect Square." };
                await smtp.SendMailAsync(message);
                return true;
            }
            catch (System.Exception ex) { _logger.LogWarning(ex, "Unable to send onboarding email to {Email}", email); return false; }
        }
    }
}
