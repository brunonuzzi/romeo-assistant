using Microsoft.Extensions.Options;
using romeo_assistant_api.Middlewares;
using romeo_assistant_core;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Services.Whatsapp;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Update the configuration builder to read from the other project's configuration file
builder.Configuration.SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("config.json", optional: false, reloadOnChange: true);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var secrets = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Services.AddRomeoCoreServices();

builder.Services.AddControllers(options => { options.InputFormatters.Add(new TextPlainInputFormatter()); });
builder.Services.AddHttpClient(); // Add this line

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var serviceProvider = serviceScope.ServiceProvider;

// Validate services configuration and api keys
var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
ValidationConfiguration.Validate(secrets,appSettings);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMiddleware<LogRequestMiddleware>();

    // Resolve the IWhatsappService instance and call the RunOnce method
    var whatsappService = serviceProvider.GetRequiredService<IWhatsappService>();

    var localUrl = $"{Environment.GetEnvironmentVariable("VS_TUNNEL_URL")}Whatsapp";
    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VS_TUNNEL_URL")))
    {
        throw new InvalidOperationException("Missing dev tunnel configuration");
    }
    whatsappService.ConfigureWebHook(localUrl);
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();