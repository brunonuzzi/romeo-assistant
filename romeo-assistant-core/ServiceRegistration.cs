using Microsoft.Extensions.DependencyInjection;
using romeo_assistant_core.Services.Behaviour.CommandHandler.Interface;
using romeo_assistant_core.Services.Behaviour.CommandHandler;
using romeo_assistant_core.Services.Behaviour;
using romeo_assistant_core.Services.ChatBot;
using romeo_assistant_core.Services.Database;
using romeo_assistant_core.Services.NextBike;
using romeo_assistant_core.Services.Whatsapp;

namespace romeo_assistant_core
{
    public static class ServiceRegistration
    {
        public static void AddRomeoCoreServices(this IServiceCollection services)
        {

            services.AddScoped(typeof(ActiveModeCommandHandler));
            services.AddScoped(typeof(RemainingTokenCommandHandler));
            services.AddScoped(typeof(PromptCommandHandler));
            services.AddScoped(typeof(LocationCommandHandler));
            services.AddScoped<Func<string, ICommandHandler>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "ActiveMode":
                        return serviceProvider.GetRequiredService<ActiveModeCommandHandler>();
                    case "RemainingToken":
                        return serviceProvider.GetRequiredService<RemainingTokenCommandHandler>();
                    case "Prompt":
                        return serviceProvider.GetRequiredService<PromptCommandHandler>();
                    case "Location":
                        return serviceProvider.GetRequiredService<LocationCommandHandler>();
                    default:
                        throw new KeyNotFoundException(); // or return null, or a default implementation
                }
            });

            services.AddScoped<ISupabaseService, SupabaseService>();
            services.AddScoped<IChatBotService, ChatBotService>();
            services.AddScoped<IWhatsappService, WhatsappService>();
            services.AddScoped<INextBikeService, NextBikeService>();
            services.AddScoped<IBehaviour, RomeoService>();
        }
    }
}
