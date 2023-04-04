using romeo_assistant_api.Middlewares;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Services.Behaviour;
using romeo_assistant_core.Services.ChatBot;
using romeo_assistant_core.Services.Database;
using romeo_assistant_core.Services.NextBike;
using romeo_assistant_core.Services.Whatsapp;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Services.AddScoped<ISupabaseService, SupabaseService>();
builder.Services.AddScoped<IChatBotService, ChatBotService>();
builder.Services.AddScoped<IWhatsappService, WhatsappService>();
builder.Services.AddScoped<INextBikeService, NextBikeService>();
builder.Services.AddScoped<IBehaviour, RomeoService>();

builder.Services.AddControllers(options => { options.InputFormatters.Add(new TextPlainInputFormatter()); });

builder.Services.AddHttpClient(); // Add this line

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMiddleware<LogRequestMiddleware>();

    // Resolve the IWhatsappService instance and call the RunOnce method
    using var serviceScope = app.Services.CreateScope();

    var serviceProvider = serviceScope.ServiceProvider;
    var whatsappService = serviceProvider.GetRequiredService<IWhatsappService>();
    var localUrl = $"{Environment.GetEnvironmentVariable("VS_TUNNEL_URL")}Whatsapp";
    whatsappService.ConfigureWebHook(localUrl);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();