using romeo_assistant.Models.Whatsapp;

namespace romeo_assistant.Services.Whatsapp
{
    public interface IWhatsappService
    {
        Task InformGroupAboutPromptReset(IncomingMessage incomingMessage);
        Task SendGroupResponse(IncomingMessage incomingMessage, string response);
        Task ConfirmGroupAboutNewPrompt(IncomingMessage incomingMessage);
        Task ConfigureWebHook();
    }
}
