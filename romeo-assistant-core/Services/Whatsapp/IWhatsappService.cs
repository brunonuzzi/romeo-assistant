using romeo_assistant_core.Models.Whatsapp;

namespace romeo_assistant_core.Services.Whatsapp
{
    public interface IWhatsappService
    {
        Task SendGroupResponse(IncomingMessage incomingMessage, string response);
        Task SendGroupMessage(IncomingMessage incomingMessage, string message);
        Task SendGroupLocationMessage(IncomingMessage incomingMessage, LocationMessage response);
        Task ConfigureWebHook(string url);
    }
}
