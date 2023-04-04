using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Models.Whatsapp;
using System.Text;

namespace romeo_assistant_core.Services.Whatsapp
{
    public class WhatsappService : IWhatsappService
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public WhatsappService(IOptions<AppSettings> appSettings, IHttpClientFactory httpClientFactory)
        {
            _appSettings = appSettings;
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendGroupResponse(IncomingMessage incomingMessage, string response)
        {
            var payload = new
            {
                message = response,
                type = "text",
                to_number = incomingMessage.Conversation,
                reply_to = incomingMessage.Message?.Id,
                skip_filter = true
            };

            await PostMessageToMayTapi(incomingMessage.Reply!, payload);
        }

        public async Task SendGroupMessage(IncomingMessage incomingMessage, string message)
        {
            var payload = new
            {
                message,
                type = "text",
                to_number = incomingMessage.Conversation,
                skip_filter = true
            };

            await PostMessageToMayTapi(incomingMessage.Reply!, payload);
        }

        public async Task SendGroupLocationMessage(IncomingMessage incomingMessage, LocationMessage response)
        {
            var payload = new
            {
                text = response.Title,
                type = "location",
                latitude = response.Lat,
                longitude = response.Lng,
                to_number = incomingMessage.Conversation,
                reply_to = incomingMessage.Message?.Id,
                skip_filter = true
            };

            await PostMessageToMayTapi(incomingMessage.Reply!, payload);
        }

        public async Task ConfigureWebHook(string url)
        {
            var payload = new
            {
                webhook = url,
                ack_delivery = false,
            };
            var httpContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-maytapi-key", _appSettings.Value.Maytapi?.ApiKey);
            var baseUrl = $"{_appSettings.Value.Maytapi?.BaseApiUrl}{_appSettings.Value.Maytapi?.ProductId}/setWebhook";

            await client.PostAsync(baseUrl, httpContent);
        }

        private async Task PostMessageToMayTapi(string baseUrl, object payload)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-maytapi-key", _appSettings.Value.Maytapi?.ApiKey);

            var response = await client.PostAsync(baseUrl, httpContent);
            response.EnsureSuccessStatusCode();
        }
    }
}
