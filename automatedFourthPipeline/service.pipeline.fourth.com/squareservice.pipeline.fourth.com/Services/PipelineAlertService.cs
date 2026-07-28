using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace squareservice.pipeline.fourth.com.Services
{
    public interface IPipelineAlertService
    {
        Task NotifyFailureAsync(PipelineFailureAlert alert);
    }

    public sealed class PipelineFailureAlert
    {
        public string Scope { get; set; }
        public string BrandName { get; set; }
        public string StoreName { get; set; }
        public string DataType { get; set; }
        public string Status { get; set; }
        public DateTime? PeriodStartUtc { get; set; }
        public DateTime? PeriodEndUtc { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string OutputFullPath { get; set; }
        public int? FourthStatusCode { get; set; }
        public string FourthResponseBody { get; set; }
        public Exception Exception { get; set; }
    }

    public sealed class PipelineAlertService : IPipelineAlertService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PipelineAlertService> _logger;

        public PipelineAlertService(
            IConfiguration configuration,
            ILogger<PipelineAlertService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task NotifyFailureAsync(PipelineFailureAlert alert)
        {
            try
            {
                var section = _configuration.GetSection("PipelineAlerts");
                if (!section.GetValue<bool>("Enabled", false))
                {
                    return;
                }

                var toAddress = section.GetValue<string>("ToAddress");
                var fromAddress = section.GetValue<string>("FromAddress");
                var host = section.GetValue<string>("Smtp:Host");

                if (string.IsNullOrWhiteSpace(toAddress) ||
                    string.IsNullOrWhiteSpace(fromAddress) ||
                    string.IsNullOrWhiteSpace(host))
                {
                    _logger.LogWarning(
                        "Pipeline alert email is enabled but missing ToAddress, FromAddress, or Smtp:Host.");
                    return;
                }

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = BuildSubject(alert),
                    Body = BuildBody(alert),
                    IsBodyHtml = false
                };

                using var client = new SmtpClient(host, section.GetValue<int>("Smtp:Port", 587))
                {
                    EnableSsl = section.GetValue<bool>("Smtp:EnableSsl", true),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var username = section.GetValue<string>("Smtp:Username");
                var password = section.GetValue<string>("Smtp:Password");
                if (!string.IsNullOrWhiteSpace(username))
                {
                    client.Credentials = new NetworkCredential(username, password);
                }
                else
                {
                    client.UseDefaultCredentials = section.GetValue<bool>("Smtp:UseDefaultCredentials", false);
                }

                await client.SendMailAsync(message);
                _logger.LogInformation(
                    "Pipeline failure alert email sent to {ToAddress} for {Scope}.",
                    toAddress,
                    alert?.Scope);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send Square to Fourth pipeline failure alert email.");
            }
        }

        private static string BuildSubject(PipelineFailureAlert alert)
        {
            var brand = string.IsNullOrWhiteSpace(alert?.BrandName) ? "Unknown brand" : alert.BrandName;
            var store = string.IsNullOrWhiteSpace(alert?.StoreName) ? "Unknown store" : alert.StoreName;
            var dataType = string.IsNullOrWhiteSpace(alert?.DataType) ? "Pipeline" : alert.DataType;

            return $"Square to Fourth {dataType} failed - {brand} / {store}";
        }

        private static string BuildBody(PipelineFailureAlert alert)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Square to Fourth pipeline failure");
            builder.AppendLine();
            builder.AppendLine($"Scope: {alert?.Scope}");
            builder.AppendLine($"Brand: {alert?.BrandName}");
            builder.AppendLine($"Store: {alert?.StoreName}");
            builder.AppendLine($"Data type: {alert?.DataType}");
            builder.AppendLine($"Status: {alert?.Status}");
            builder.AppendLine($"Transaction date: {FormatDate(alert?.TransactionDate)}");
            builder.AppendLine($"Period start UTC: {FormatDate(alert?.PeriodStartUtc)}");
            builder.AppendLine($"Period end UTC: {FormatDate(alert?.PeriodEndUtc)}");
            builder.AppendLine($"Output file: {alert?.OutputFullPath}");
            builder.AppendLine($"Fourth status code: {alert?.FourthStatusCode}");
            builder.AppendLine();

            if (!string.IsNullOrWhiteSpace(alert?.FourthResponseBody))
            {
                builder.AppendLine("Fourth response:");
                builder.AppendLine(alert.FourthResponseBody);
                builder.AppendLine();
            }

            if (alert?.Exception != null)
            {
                builder.AppendLine("Exception:");
                builder.AppendLine(alert.Exception.ToString());
            }

            return builder.ToString();
        }

        private static string FormatDate(DateTime? dateTime)
        {
            return dateTime.HasValue
                ? dateTime.Value.ToString("O")
                : "";
        }
    }
}
