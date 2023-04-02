using romeo_assistant.Models.Whatsapp;

namespace romeo_assistant.Services.Behaviour
{
    public interface IBehaviour
    {
        Task ExecuteWorkFlow(IncomingMessage incomingMessage);
    }
}
