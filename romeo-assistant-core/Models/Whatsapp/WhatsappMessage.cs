using Newtonsoft.Json;

namespace romeo_assistant_core.Models.Whatsapp;

public class WhatsappMessage
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("text")]
    public string? Text { get; set; }

    [JsonProperty("fromMe")]
    public bool? FromMe { get; set; }

    [JsonProperty("mentions")]
    public List<string>? Mentions { get; set; }

    [JsonProperty("statuses")]
    public List<StatusMessage>? Statuses { get; set; }

    [JsonProperty("_serialized")]
    public string? Serialized { get; set; }
}