using Microsoft.Extensions.Options;
using Postgrest;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Models.Supabase;
using romeo_assistant_core.Models.Whatsapp;
using romeo_assistant_core.Utils;
using Supabase;
using Client = Supabase.Client;
using Message = romeo_assistant_core.Models.Supabase.Message;


namespace romeo_assistant_core.Services.Database
{
    public class SupabaseService : ISupabaseService
    {
        private readonly Client _supabaseClient;
        private readonly IOptions<AppSettings> _appSettings;

        public SupabaseService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _supabaseClient = new Client(_appSettings.Value.Supabase?.Url!,
                _appSettings.Value.Supabase?.Key!,
                new SupabaseOptions
                {
                    AutoConnectRealtime = true
                });
        }

        public async Task<Group?> GetGroupAsync(string groupId)
        {
            return (await _supabaseClient.Postgrest.Table<Group>()
                    .Where(x => x.GroupId == groupId)
                    .Get())
                .Models
                .FirstOrDefault();
        }

        public async Task<Group> CreateGroupAsync(IncomingMessage incomingMessage)
        {
            var result = await _supabaseClient.Postgrest.Table<Group>()
                .Insert(new Group
                {
                    GroupName = incomingMessage.Conversation_Name,
                    GroupId = incomingMessage.Conversation,
                    CreatedAt = DateTime.Now
                });

            return result.Models.First();
        }

        public async Task<Prompt?> GetPromptAsync(Group group)
        {
            return (await _supabaseClient.Postgrest.Table<Prompt>()
                    .Where(x => x.GroupId == group.Id)
                    .Order(x => x.CreatedAt, Constants.Ordering.Descending)
                    .Order(x => x.Id, Constants.Ordering.Descending)
                    .Get())
                .Models
                .FirstOrDefault();
        }

        public async Task<Prompt> CreatePromptAsync(Group group, IncomingMessage? incomingMessage = null)
        {
            var prompt = incomingMessage != null ?
                $"{incomingMessage.Message?.Text?.Replace(_appSettings.Value.RomeoSetup?.ResetPromptCommand!, "").Trim()},"
                : "";

            prompt += _appSettings.Value.RomeoSetup?.BasicPromptMessage;

            var result = await _supabaseClient.Postgrest.Table<Prompt>()
                .Insert(new Prompt
                {
                    GroupId = group.Id,
                    PromptText = prompt.Trim(),
                    TokenSize = Helper.CalculateTokenSize(prompt.Trim()),
                    CreatedAt = DateTime.Now
                });

            return result.Models.First();
        }

        public async Task<Message> SaveMessageAsync(Prompt prompt, IncomingMessage incomingMessage)
        {
            var result = await _supabaseClient.Postgrest.Table<Message>()
                .Insert(new Message
                {
                    PromptId = prompt.Id,
                    MessageType = MessageType.User,
                    MessageId = incomingMessage.Message?.Id,
                    UserId = incomingMessage.User?.Id,
                    UserName = incomingMessage.User?.Name,
                    Phone = incomingMessage.User?.Phone,
                    MessageText = incomingMessage.Message?.Text,
                    TokenSize = Helper.CalculateTokenSize(incomingMessage.Message?.Text!),
                    CreatedAt = Helper.ToLocalTime((long)incomingMessage.Timestamp!)
                });

            return result.Models.First();
        }

        public async Task<Message> SaveAIResponseAsync(Prompt prompt, string response)
        {
            var result = await _supabaseClient.Postgrest.Table<Message>()
                .Insert(new Message
                {
                    PromptId = prompt.Id,
                    MessageType = MessageType.Bot,
                    UserId = "bot",
                    UserName = "bot",
                    MessageText = response,
                    TokenSize = Helper.CalculateTokenSize(response),
                    CreatedAt = DateTime.Now
                });

            return result.Models.First();
        }

        public async Task<List<Message>> GetContextMessagesAsync(Prompt prompt)
        {
            return (await _supabaseClient.Postgrest.Table<Message>()
                    .Where(x => x.PromptId == prompt.Id)
                    .Order(x => x.CreatedAt, Constants.Ordering.Ascending)
                    .Order(x => x.Id, Constants.Ordering.Ascending)
                    .Get())
                .Models
                .ToList();
        }
    }
}
