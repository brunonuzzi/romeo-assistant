using Newtonsoft.Json;

namespace romeo_assistant_core.Models.Whatsapp;

public class User
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("phone")]
    public string? Phone { get; set; }
}