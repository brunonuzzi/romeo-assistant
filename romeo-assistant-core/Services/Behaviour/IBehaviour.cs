using romeo_assistant_core.Models.Whatsapp;

namespace romeo_assistant_core.Services.Behaviour
{
    public interface IBehaviour
    {
        Task ExecuteWorkFlow(IncomingMessage incomingMessage);
    }
}
