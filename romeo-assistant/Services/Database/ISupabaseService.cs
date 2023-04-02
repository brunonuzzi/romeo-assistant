using romeo_assistant.Models.Supabase;
using romeo_assistant.Models.Whatsapp;

namespace romeo_assistant.Services.Database
{
    public interface ISupabaseService
    {
        Task<Group?> GetGroupAsync(string incomingMessageConversation);
        Task<Group> CreateGroupAsync(IncomingMessage incomingMessage);
        Task<Prompt?> GetPromptAsync(Group group);
        Task<Prompt> CreatePromptAsync(Group group, IncomingMessage? incomingMessage = null);
        Task<Message> SaveMessageAsync(Prompt prompt, IncomingMessage incomingMessage);
        Task<Message> SaveAIResponseAsync(Prompt prompt, string response);
        Task<List<Message>> GetContextMessagesAsync(Prompt prompt);
    }
}
