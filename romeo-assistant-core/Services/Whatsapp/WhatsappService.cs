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

        public WhatsappService(IOptions<AppSettings> appSettings, HttpClient httpClient, IHttpClientFactory httpClientFactory)
        {
            _appSettings = appSettings;
            _httpClientFactory = httpClientFactory;
        }

        public async Task InformGroupAboutPromptReset(IncomingMessage incomingMessage)
        {
            await SendMessageAsync(incomingMessage.Reply!, _appSettings.Value.RomeoSetup?.PromptResetMessage!, incomingMessage.Conversation!);
        }

        public async Task SendGroupResponse(IncomingMessage incomingMessage, string response)
        {
            await SendMessageAsync(incomingMessage.Reply!, response, incomingMessage.Conversation!, incomingMessage.Message?.Id!);
        }

        public async Task ConfirmGroupAboutNewPrompt(IncomingMessage incomingMessage)
        {
            await SendMessageAsync(incomingMessage.Reply!, _appSettings.Value.RomeoSetup?.ResetPromptSuccess!, incomingMessage.Conversation!, incomingMessage.Message?.Id!);
        }

        public async Task ConfigureWebHook()
        {
            var localUrl = $"{Environment.GetEnvironmentVariable("VS_TUNNEL_URL")}Whatsapp";
            var payload = new
            {
                webhook = localUrl,
                ack_delivery = false,
            };
            var httpContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-maytapi-key", _appSettings.Value.Maytapi?.ApiKey);
            var baseUrl = $"{_appSettings.Value.Maytapi?.BaseApiUrl}{_appSettings.Value.Maytapi?.ProductId}/setWebhook";

            var x = await client.PostAsync(baseUrl, httpContent);

        }

        private async Task SendMessageAsync(string baseUrl, string message, string toNumber, string replyTo)
        {
            var payload = new
            {
                message,
                type = "text",
                to_number = toNumber,
                reply_to = replyTo,
                skip_filter = true
            };

            await PostMessageToMayTapi(baseUrl, payload);
        }

        private async Task SendMessageAsync(string baseUrl, string message, string toNumber)
        {
            var payload = new
            {
                message,
                type = "text",
                to_number = toNumber,
                skip_filter = true
            };

            await PostMessageToMayTapi(baseUrl, payload);
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
