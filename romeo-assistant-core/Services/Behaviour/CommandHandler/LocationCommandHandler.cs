using System.Globalization;
using romeo_assistant_core.Models.Supabase;
using romeo_assistant_core.Models.Whatsapp;
using romeo_assistant_core.Services.Behaviour.CommandHandler.Interface;
using romeo_assistant_core.Services.NextBike;
using romeo_assistant_core.Services.Whatsapp;

namespace romeo_assistant_core.Services.Behaviour.CommandHandler;

public class LocationCommandHandler : ICommandHandler
{
    private readonly IWhatsappService _whatsappService;
    private readonly INextBikeService _nextBikeService;

    public LocationCommandHandler(IWhatsappService whatsappService, INextBikeService nextBikeService)
    {
        _whatsappService = whatsappService;
        _nextBikeService = nextBikeService;
    }

    public async Task<bool> HandleCommandAsync(IncomingMessage incomingMessage, Group group, Prompt prompt)
    {
        if (incomingMessage?.Message?.Type == "location")
        {
            var response = incomingMessage.Message.Payload;

            var lat = Convert.ToDouble(response!.Split(",")[0], CultureInfo.InvariantCulture);
            var lng = Convert.ToDouble(response.Split(",")[1], CultureInfo.InvariantCulture);

            var locationMessage = await _nextBikeService.GetNextBikeDataByLocationAsync(lat, lng);

            await _whatsappService.SendGroupResponse(incomingMessage, locationMessage.Text!);
            await _whatsappService.SendGroupLocationMessage(incomingMessage, locationMessage);

            return true;
        }

        return false;
    }
}