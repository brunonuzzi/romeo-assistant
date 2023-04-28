using Microsoft.Extensions.Options;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Models.Supabase;
using romeo_assistant_core.Models.Whatsapp;
using romeo_assistant_core.Services.Behaviour.CommandHandler;
using romeo_assistant_core.Services.Behaviour.CommandHandler.Interface;
using romeo_assistant_core.Services.ChatBot;
using romeo_assistant_core.Services.Database;
using romeo_assistant_core.Services.Whatsapp;

namespace romeo_assistant_core.Services.Behaviour
{
    public class RomeoService : IBehaviour
    {
        private readonly ISupabaseService _supabaseService;
        private readonly IWhatsappService _whatsappService;
        private readonly IChatBotService _chatBotService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly List<ICommandHandler> _commandHandlers;

        public RomeoService(IWhatsappService whatsappService,
            ISupabaseService supabaseService,
            IChatBotService chatBotService,
            IOptions<AppSettings> appSettings,
            ActiveModeCommandHandler activeModeCommandHandler,
            LocationCommandHandler locationCommandHandler,
            RemainingTokenCommandHandler remainingTokenCommandHandler,
            PromptCommandHandler promptCommandHandler
        )
        {
            _whatsappService = whatsappService;
            _supabaseService = supabaseService;
            _chatBotService = chatBotService;
            _appSettings = appSettings;
            _commandHandlers = new List<ICommandHandler>
            {
                activeModeCommandHandler,
                locationCommandHandler,
                remainingTokenCommandHandler,
                promptCommandHandler
            };
        }

        public async Task ExecuteWorkFlow(IncomingMessage incomingMessage)
        {
            if (IsATextMessageOrLocationForAnyGroup(incomingMessage))
            {
                var group = await GetOrCreateGroupAsync(incomingMessage);
                var prompt = await GetOrCreatePromptAsync(group);

                if (await HandleCommands(incomingMessage, group, prompt)) return;

                await SaveMessageAndHandlePrompt(incomingMessage, group, prompt);
            }
        }

        private async Task<Group> GetOrCreateGroupAsync(IncomingMessage incomingMessage)
        {
            return await _supabaseService.GetGroupAsync(incomingMessage.Conversation!)
                   ?? await _supabaseService.CreateGroupAsync(incomingMessage);
        }

        private async Task<Prompt> GetOrCreatePromptAsync(Group group)
        {
            return await _supabaseService.GetPromptAsync(group)
                   ?? await _supabaseService.CreatePromptAsync(group);
        }

        private async Task<bool> HandleCommands(IncomingMessage incomingMessage, Group group, Prompt prompt)
        {
            foreach (var commandHandler in _commandHandlers)
            {
                var handled = await commandHandler.HandleCommandAsync(incomingMessage, group, prompt);
                if (handled) return true;
            }
            return false;
        }

        private async Task SaveMessageAndHandlePrompt(IncomingMessage incomingMessage, Group group, Prompt prompt)
        {
            await _supabaseService.SaveMessageAsync(prompt, incomingMessage);

            var contextMessages = await _supabaseService.GetContextMessagesAsync(prompt);

            if (contextMessages.Sum(x => x.TokenSize) + prompt.TokenSize >= _appSettings.Value.RomeoSetup?.TokenMaxSize)
            {
                if (group.ActiveMode)
                {
                    await InformGroupAboutPromptReset(incomingMessage);
                }
                prompt = await _supabaseService.CreatePromptAsync(group);
                await _supabaseService.SaveMessageAsync(prompt, incomingMessage);
                contextMessages = await _supabaseService.GetContextMessagesAsync(prompt);
            }

            if (group.ActiveMode || HasAnyGroupMention(incomingMessage))
            {
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

        private bool HasAnyGroupMention(IncomingMessage incomingMessage) =>
            (incomingMessage?.Message?.Text?.Contains(_appSettings.Value.RomeoSetup?.RomeoNumber!, StringComparison.InvariantCultureIgnoreCase) ?? false)
            || (incomingMessage?.Quoted?.User?.Phone?.Contains(_appSettings.Value.RomeoSetup?.RomeoNumber!, StringComparison.InvariantCultureIgnoreCase) ?? false)
            || (incomingMessage?.Message!.Text?.Contains(_appSettings.Value.RomeoSetup?.RomeoName!, StringComparison.InvariantCultureIgnoreCase) ?? false);

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
