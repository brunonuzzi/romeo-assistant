using romeo_assistant_core.Models.Supabase;

namespace romeo_assistant_core.Services.ChatBot
{
    public interface IChatBotService
    {
        Task<string> GenerateResponseAsync(List<Message> contextMessages, Prompt prompt);
    }
}
