using Microsoft.Extensions.Options;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Models.Supabase;
using romeo_assistant_core.Models.Whatsapp;
using romeo_assistant_core.Services.ChatBot;
using romeo_assistant_core.Services.Database;
using romeo_assistant_core.Services.NextBike;
using romeo_assistant_core.Services.Whatsapp;
using System.Globalization;

namespace romeo_assistant_core.Services.Behaviour
{
    public class RomeoService : IBehaviour
    {
        private readonly ISupabaseService _supabaseService;
        private readonly IWhatsappService _whatsappService;
        private readonly IChatBotService _chatBotService;
        private readonly INextBikeService _nextBikeService;
        private readonly IOptions<AppSettings> _appSettings;

        public RomeoService(IWhatsappService whatsappService,
            ISupabaseService supabaseService,
            IChatBotService chatBotService,
            INextBikeService nextBikeService,
            IOptions<AppSettings> appSettings)
        {
            _whatsappService = whatsappService;
            _supabaseService = supabaseService;
            _chatBotService = chatBotService;
            _nextBikeService = nextBikeService;
            _appSettings = appSettings;
        }

        public async Task ExecuteWorkFlow(IncomingMessage incomingMessage)
        {
            if (IsATextMessageOrLocationForAnyGroup(incomingMessage))
            {
                var group = await _supabaseService.GetGroupAsync(incomingMessage.Conversation!)
                            ?? await _supabaseService.CreateGroupAsync(incomingMessage);

                if (await HasPromptCommand(incomingMessage, group)) return;

                var prompt = await _supabaseService.GetPromptAsync(group)
                             ?? await _supabaseService.CreatePromptAsync(group);

                if (await HasRemainingTokenCommand(incomingMessage, prompt)) return;
                if (await HasLocationMessage(incomingMessage, prompt)) return;

                await _supabaseService.SaveMessageAsync(prompt, incomingMessage);

                var contextMessages = await _supabaseService.GetContextMessagesAsync(prompt);

                if (contextMessages.Sum(x => x.TokenSize) + prompt.TokenSize >= _appSettings.Value.RomeoSetup?.TokenMaxSize)
                {
                    await InformGroupAboutPromptReset(incomingMessage);
                    prompt = await _supabaseService.CreatePromptAsync(group);
                    await _supabaseService.SaveMessageAsync(prompt, incomingMessage);
                    contextMessages = await _supabaseService.GetContextMessagesAsync(prompt);
                }

                var response = string.Empty;
                try
                {
                    response = await _chatBotService.GenerateResponseAsync(contextMessages, prompt);
                }
                catch (Exception)
                {
                    await InformGroupAboutPromptReset(incomingMessage);
                    prompt = await _supabaseService.CreatePromptAsync(group);
                    await _supabaseService.SaveMessageAsync(prompt, incomingMessage);
                }

                await _supabaseService.SaveAIResponseAsync(prompt, response);
                await _whatsappService.SendGroupResponse(incomingMessage, response);
            }
        }

        private async Task<bool> HasLocationMessage(IncomingMessage incomingMessage, Prompt prompt)
        {
            if (incomingMessage?.Message?.Type == "location")
            {
                var response = incomingMessage.Message.Payload;

                var lat = Convert.ToDouble(response!.Split(",")[0], CultureInfo.InvariantCulture);
                var lng = Convert.ToDouble(response.Split(",")[1], CultureInfo.InvariantCulture);

                var locationMessage = await _nextBikeService.GetNextBikeDataByLocationAsync(lat, lng);

                await _supabaseService.SaveAIResponseAsync(prompt, locationMessage.Text!);
                await _whatsappService.SendGroupResponse(incomingMessage, locationMessage.Text!);
                await _whatsappService.SendGroupLocationMessage(incomingMessage, locationMessage);

                return true;
            }

            return false;
        }

        private async Task<bool> HasRemainingTokenCommand(IncomingMessage incomingMessage, Prompt prompt)
        {
            if (incomingMessage.Message?.Text != null && incomingMessage.Message!.Text!.Contains(_appSettings.Value.RomeoSetup?.TokensRemainingCommand!))
            {
                var tokenMaxSize = _appSettings.Value.RomeoSetup!.TokenMaxSize;
                var tokensRemainingMessage = _appSettings.Value.RomeoSetup?.TokensRemainingSuccess!;

                var contextMessages = await _supabaseService.GetContextMessagesAsync(prompt);
                var tokensRemainingCount = tokenMaxSize - (contextMessages.Sum(x => x.TokenSize) + prompt.TokenSize);
                var response = string.Format(tokensRemainingMessage, tokensRemainingCount, tokenMaxSize);

                await _supabaseService.SaveAIResponseAsync(prompt, response);
                await _whatsappService.SendGroupMessage(incomingMessage, response);

                return true;
            }

            return false;
        }

        private async Task<bool> HasPromptCommand(IncomingMessage incomingMessage, Group group)
        {
            if (incomingMessage.Message?.Text != null && incomingMessage.Message.Text.Contains(_appSettings.Value.RomeoSetup?.ResetPromptCommand!))
            {
                await _supabaseService.CreatePromptAsync(group, incomingMessage);
                await _whatsappService.SendGroupMessage(incomingMessage, _appSettings.Value.RomeoSetup?.ResetPromptSuccess!);

                return true;
            }

            return false;
        }


        private async Task InformGroupAboutPromptReset(IncomingMessage incomingMessage) =>
            await _whatsappService.SendGroupMessage(incomingMessage, _appSettings.Value.RomeoSetup?.PromptResetMessage!);

        private static bool IsATextMessageOrLocationForAnyGroup(IncomingMessage incomingMessage) =>
            incomingMessage?.Type == "message"
            && (incomingMessage?.Message?.Type == "text" || incomingMessage?.Message?.Type == "location")
            && (incomingMessage?.Message?.Text != null || incomingMessage?.Message?.Payload != null)
            && incomingMessage?.Message.FromMe != true
            && incomingMessage!.Conversation!.Contains("@g.us");
    }
}
