using Microsoft.Extensions.Options;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Models.Supabase;
using romeo_assistant_core.Models.Whatsapp;
using romeo_assistant_core.Services.Behaviour.CommandHandler.Interface;
using romeo_assistant_core.Services.Database;
using romeo_assistant_core.Services.Whatsapp;

namespace romeo_assistant_core.Services.Behaviour.CommandHandler;

public class RemainingTokenCommandHandler : ICommandHandler
{
    private readonly ISupabaseService _supabaseService;
    private readonly IWhatsappService _whatsappService;
    private readonly IOptions<AppSettings> _appSettings;

    public RemainingTokenCommandHandler(ISupabaseService supabaseService, IWhatsappService whatsappService, IOptions<AppSettings> appSettings)
    {
        _supabaseService = supabaseService;
        _whatsappService = whatsappService;
        _appSettings = appSettings;
    }

    public async Task<bool> HandleCommandAsync(IncomingMessage incomingMessage, Group group, Prompt prompt)
    {
        if (incomingMessage.Message?.Text != null && incomingMessage.Message!.Text!.Contains(_appSettings.Value.RomeoSetup?.TokensRemainingCommand!))
        {
            var tokenMaxSize = _appSettings.Value.RomeoSetup!.TokenMaxSize;
            var tokensRemainingMessage = _appSettings.Value.RomeoSetup?.TokensRemainingSuccess!;

            var contextMessages = await _supabaseService.GetContextMessagesAsync(prompt);
            var tokensRemainingCount = tokenMaxSize - (contextMessages.Sum(x => x.TokenSize) + prompt.TokenSize);
            var response = string.Format(tokensRemainingMessage, tokensRemainingCount, tokenMaxSize);

            await _whatsappService.SendGroupMessage(incomingMessage, response);
            return true;
        }

        return false;
    }
}