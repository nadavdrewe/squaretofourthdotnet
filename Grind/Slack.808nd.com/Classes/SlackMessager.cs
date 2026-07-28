using System;
using System.Net.Http;
using System.Threading.Tasks;
using Slack._808nd.com.Interfaces;

namespace Slack._808nd.com.Classes
{
    public class SlackMessenger : ISlackMessager
    {
        private readonly HttpClient theClient = new HttpClient();

        public async Task<string> SendMessage(string message, string channel, string username)
        {
            var token = Environment.GetEnvironmentVariable("SLACK_API_TOKEN");
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("SLACK_API_TOKEN must be configured before sending Slack messages.");
            }

            var fullUri = "https://slack.com/api/chat.postMessage?token=" + Uri.EscapeDataString(token);
            fullUri += "&username=" + Uri.EscapeDataString(username);
            fullUri += "&channel=%23" + Uri.EscapeDataString(channel);
            fullUri += "&text=" + Uri.EscapeDataString(message);

            var response = await theClient.GetAsync(fullUri);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
