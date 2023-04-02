using Microsoft.Extensions.Options;
using romeo_assistant.Models.Configuration;
using romeo_assistant.Models.Supabase;
using romeo_assistant.Models.Whatsapp;
using romeo_assistant.Services.ChatBot;
using romeo_assistant.Services.Database;
using romeo_assistant.Services.Whatsapp;

namespace romeo_assistant.Services.Behaviour
{
    public class RomeoService : IBehaviour
    {
        private readonly ISupabaseService _supabaseService;
        private readonly IWhatsappService _whatsappService;
        private readonly IChatBotService _chatBotService;
        private readonly IOptions<AppSettings> _appSettings;

        public RomeoService(IWhatsappService whatsappService,
            ISupabaseService supabaseService,
            IChatBotService chatBotService,
            IOptions<AppSettings> appSettings)
        {
            _whatsappService = whatsappService;
            _supabaseService = supabaseService;
            _chatBotService = chatBotService;
            _appSettings = appSettings;
        }

        public async Task ExecuteWorkFlow(IncomingMessage incomingMessage)
        {
            if (IsATextMessageForAnyGroup(incomingMessage))
            {
                var group = await _supabaseService.GetGroupAsync(incomingMessage.Conversation!)
                            ?? await _supabaseService.CreateGroupAsync(incomingMessage);

                if (await HasPromptCommand(incomingMessage, group)) return;

                var prompt = await _supabaseService.GetPromptAsync(group)
                             ?? await _supabaseService.CreatePromptAsync(group);

                await _supabaseService.SaveMessageAsync(prompt, incomingMessage);

                var contextMessages = await _supabaseService.GetContextMessagesAsync(prompt);

                if (contextMessages.Sum(x => x.TokenSize) + prompt.TokenSize >= _appSettings.Value.RomeoSetup?.TokenMaxSize)
                {
                    await _whatsappService.InformGroupAboutPromptReset(incomingMessage);
                    prompt = await _supabaseService.CreatePromptAsync(group);
                    await _supabaseService.SaveMessageAsync(prompt, incomingMessage);
                    contextMessages = await _supabaseService.GetContextMessagesAsync(prompt);
                }

                var response = await _chatBotService.GenerateResponseAsync(contextMessages, prompt);

                await _supabaseService.SaveAIResponseAsync(prompt, response);

                await _whatsappService.SendGroupResponse(incomingMessage, response);
            }
        }

        private async Task<bool> HasPromptCommand(IncomingMessage incomingMessage, Group group)
        {
            if (incomingMessage.Message.Text.Contains(_appSettings.Value.RomeoSetup?.ResetPromptCommand!))
            {
                await _supabaseService.CreatePromptAsync(group, incomingMessage);
                await _whatsappService.ConfirmGroupAboutNewPrompt(incomingMessage);

                return true;
            }

            return false;
        }

        private static bool IsATextMessageForAnyGroup(IncomingMessage incomingMessage) =>
            incomingMessage.Type == "message" && incomingMessage.Message?.Text != null && incomingMessage.Message.FromMe != true && incomingMessage.Conversation!.Contains("@g.us");
    }
}
