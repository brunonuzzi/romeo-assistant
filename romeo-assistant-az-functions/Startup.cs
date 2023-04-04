using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Services.Behaviour;
using romeo_assistant_core.Services.ChatBot;
using romeo_assistant_core.Services.Database;
using romeo_assistant_core.Services.Whatsapp;

[assembly: FunctionsStartup(typeof(romeo_assistant_az_functions.Startup))]

namespace romeo_assistant_az_functions
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(builder.GetContext().ApplicationRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton(config);
            builder.Services.Configure<AppSettings>(config);
            builder.Services.AddScoped<ISupabaseService, SupabaseService>();
            builder.Services.AddScoped<IChatBotService, ChatBotService>();
            builder.Services.AddScoped<IWhatsappService, WhatsappService>();
            builder.Services.AddScoped<IBehaviour, RomeoService>();
        }
    }
}
