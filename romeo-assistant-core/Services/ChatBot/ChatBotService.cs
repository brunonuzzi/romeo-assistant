using Microsoft.Extensions.Options;
using OpenAI_API;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Models.Supabase;

namespace romeo_assistant_core.Services.ChatBot
{
    public class ChatBotService : IChatBotService
    {
        private readonly OpenAIAPI _openAiapi;

        public ChatBotService(IOptions<AppSettings> appSettings)
        {
            _openAiapi = new OpenAIAPI(appSettings.Value.OpenAi?.ApiKey);
        }

        public async Task<string> GenerateResponseAsync(List<Message> contextMessages, Prompt prompt)
        {
            var chat = _openAiapi.Chat.CreateConversation();
            chat.AppendSystemMessage(prompt.PromptText);

            foreach (var contextMessage in contextMessages)
            {
                switch (contextMessage.MessageType)
                {
                    case MessageType.Bot:
                        chat.AppendExampleChatbotOutput($"{contextMessage.MessageText}");
                        break;

                    case MessageType.User:
                        chat.AppendUserInput($"{contextMessage.UserName}:{contextMessage.MessageText}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return await chat.GetResponseFromChatbotAsync();
        }
    }
}
