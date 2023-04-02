using romeo_assistant.Models.Supabase;

namespace romeo_assistant.Services.ChatBot
{
    public interface IChatBotService
    {
        Task<string> GenerateResponseAsync(List<Message> contextMessages, Prompt prompt);
    }
}
