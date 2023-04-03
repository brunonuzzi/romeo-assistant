using romeo_assistant_core.Models.Whatsapp;

namespace romeo_assistant_core.Services.Whatsapp
{
    public interface IWhatsappService
    {
        Task InformGroupAboutPromptReset(IncomingMessage incomingMessage);
        Task SendGroupResponse(IncomingMessage incomingMessage, string response);
        Task ConfirmGroupAboutNewPrompt(IncomingMessage incomingMessage);
        Task ConfigureWebHook();
    }
}
