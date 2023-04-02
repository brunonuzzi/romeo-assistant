using Newtonsoft.Json;

namespace romeo_assistant.Models.Whatsapp;

public class StatusMessage
{
    [JsonProperty("status")]
    public string? Status { get; set; }
}