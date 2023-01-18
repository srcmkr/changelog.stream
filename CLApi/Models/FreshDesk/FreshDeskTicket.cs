using Newtonsoft.Json;

namespace CLApi.Models.FreshDesk;

public class FreshDeskTicket
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("subject")]
    public string Subject { get; set; }
    
    [JsonProperty("custom_fields")]
    public Dictionary<string, string> CustomFields { get; set; }
    
    [JsonProperty("type")]
    public string Type { get; set; }
}