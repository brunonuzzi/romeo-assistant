using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using romeo_assistant_core;
using romeo_assistant_core.Models.Configuration;
using System.Reflection;

[assembly: FunctionsStartup(typeof(romeo_assistant_az_functions.Startup))]

namespace romeo_assistant_az_functions
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(builder.GetContext().ApplicationRootPath)
                .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .AddEnvironmentVariables()
                .Build();
            
            var appSettings = config.Get<AppSettings>();

            ValidationConfiguration.Validate(config,appSettings);

            builder.Services.AddRomeoCoreServices();
        }
    }
}
