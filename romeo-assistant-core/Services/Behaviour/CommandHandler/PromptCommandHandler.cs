using Microsoft.Extensions.Options;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Models.Supabase;
using romeo_assistant_core.Models.Whatsapp;
using romeo_assistant_core.Services.Behaviour.CommandHandler.Interface;
using romeo_assistant_core.Services.Database;
using romeo_assistant_core.Services.Whatsapp;

namespace romeo_assistant_core.Services.Behaviour.CommandHandler;

public class PromptCommandHandler : ICommandHandler
{
    private readonly ISupabaseService _supabaseService;
    private readonly IWhatsappService _whatsappService;
    private readonly IOptions<AppSettings> _appSettings;

    public PromptCommandHandler(ISupabaseService supabaseService, IWhatsappService whatsappService, IOptions<AppSettings> appSettings)
    {
        _supabaseService = supabaseService;
        _whatsappService = whatsappService;
        _appSettings = appSettings;
    }

    public async Task<bool> HandleCommandAsync(IncomingMessage incomingMessage, Group group, Prompt prompt)
    {
        if (incomingMessage.Message?.Text != null && incomingMessage.Message.Text.Contains(_appSettings.Value.RomeoSetup?.ResetPromptCommand!))
        {
            await _supabaseService.CreatePromptAsync(group, incomingMessage);
            await _whatsappService.SendGroupMessage(incomingMessage, _appSettings.Value.RomeoSetup?.ResetPromptSuccess!);

            return true;
        }

        return false;
    }
}