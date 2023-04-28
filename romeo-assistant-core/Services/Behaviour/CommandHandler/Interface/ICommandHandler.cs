using romeo_assistant_core.Models.Supabase;
using romeo_assistant_core.Models.Whatsapp;

namespace romeo_assistant_core.Services.Behaviour.CommandHandler.Interface;

public interface ICommandHandler
{
    Task<bool> HandleCommandAsync(IncomingMessage incomingMessage, Group group, Prompt prompt);
}