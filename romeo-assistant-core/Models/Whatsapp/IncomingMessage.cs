using Newtonsoft.Json;

namespace romeo_assistant_core.Models.Whatsapp;

public class IncomingMessage
{
    [JsonProperty("productId")]
    public Guid? ProductId { get; set; }

    [JsonProperty("phoneId")]
    public int? PhoneId { get; set; }

    [JsonProperty("message")]
    public WhatsappMessage? Message { get; set; }

    [JsonProperty("user")]
    public User? User { get; set; }

    [JsonProperty("conversation")]
    public string? Conversation { get; set; }

    [JsonProperty("conversation_name")]
    public string? Conversation_Name { get; set; }

    [JsonProperty("participants")]
    public List<User>? Participants { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("receiver")]
    public string? Receiver { get; set; }

    [JsonProperty("timestamp")]
    public int? Timestamp { get; set; }

    [JsonProperty("reply")]
    public string? Reply { get; set; }

    [JsonProperty("quoted")]
    public WhatsappMessage? Quoted { get; set; }
}