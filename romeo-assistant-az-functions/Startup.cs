using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Services.Behaviour;
using romeo_assistant_core.Services.ChatBot;
using romeo_assistant_core.Services.Database;
using romeo_assistant_core.Services.Whatsapp;
using System;

[assembly: FunctionsStartup(typeof(romeo_assistant_az_functions.Startup))]

namespace romeo_assistant_az_functions
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton(config);
            builder.Services.Configure<AppSettings>(config);
            builder.Services.AddScoped<ISupabaseService, SupabaseService>();
            builder.Services.AddScoped<IChatBotService, ChatBotService>();
            builder.Services.AddScoped<IWhatsappService, WhatsappService>();
            builder.Services.AddScoped<IBehaviour, RomeoService>();

            //var serviceProvider = builder.Services.BuildServiceProvider();
            //var appSettings = serviceProvider.GetService<AppSettings>();
            //    whatsappService.ConfigureWebHook(appSettings?.RomeoSetup?.AzureFunctionsUrl!);
            //var environment = config.GetValue<string>("ASPNETCORE_ENVIRONMENT");
            //var isReleaseVersion = environment?.ToLower() == "release";
            //if (isReleaseVersion)
            //{
            //    var serviceProvider = builder.Services.BuildServiceProvider();
            //    var whatsappService = serviceProvider.GetService<IWhatsappService>();
            //    var appSettings = serviceProvider.GetService<AppSettings>();
            //    whatsappService.ConfigureWebHook(appSettings?.RomeoSetup?.AzureFunctionsUrl!);
            //}
        }
    }
}
