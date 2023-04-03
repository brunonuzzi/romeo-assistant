using Newtonsoft.Json;

namespace romeo_assistant_core.Models.Whatsapp;

public class StatusMessage
{
    [JsonProperty("status")]
    public string? Status { get; set; }
}