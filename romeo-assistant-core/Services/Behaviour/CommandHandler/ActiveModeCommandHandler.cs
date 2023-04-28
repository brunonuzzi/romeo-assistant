using Microsoft.Extensions.Options;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Models.Supabase;
using romeo_assistant_core.Models.Whatsapp;
using romeo_assistant_core.Services.Behaviour.CommandHandler.Interface;
using romeo_assistant_core.Services.Database;
using romeo_assistant_core.Services.Whatsapp;

namespace romeo_assistant_core.Services.Behaviour.CommandHandler;

public class ActiveModeCommandHandler : ICommandHandler
{
    private readonly ISupabaseService _supabaseService;
    private readonly IWhatsappService _whatsappService;
    private readonly IOptions<AppSettings> _appSettings;

    public ActiveModeCommandHandler(ISupabaseService supabaseService, IWhatsappService whatsappService, IOptions<AppSettings> appSettings)
    {
        _supabaseService = supabaseService;
        _whatsappService = whatsappService;
        _appSettings = appSettings;
    }

    public async Task<bool> HandleCommandAsync(IncomingMessage incomingMessage, Group group, Prompt prompt)
    {
        if (incomingMessage.Message?.Text != null
            && (incomingMessage.Message?.Text?.Contains(_appSettings.Value.RomeoSetup?.ActiveCommand!) ?? false)
            || (incomingMessage.Message?.Text?.Contains(_appSettings.Value.RomeoSetup?.PassiveCommand!) ?? false))
        {
            var isActiveMode = incomingMessage.Message!.Text!.Contains(_appSettings.Value.RomeoSetup?.ActiveCommand!);
            await _supabaseService.UpdateGroupActiveMode(group, isActiveMode);

            var msg = isActiveMode ? _appSettings.Value.RomeoSetup?.ActiveCommandMessage
                : _appSettings.Value.RomeoSetup?.PassiveCommandMessage;

            await _whatsappService.SendGroupMessage(incomingMessage, msg);

            return true;
        }

        return false;
    }
}