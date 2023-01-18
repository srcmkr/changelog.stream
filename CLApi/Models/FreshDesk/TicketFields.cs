using Newtonsoft.Json;

namespace CLApi.Models.FreshDesk;

public class TicketFields
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("label_for_customers")]
    public string LabelForCustomers { get; set; }
    
    [JsonProperty("type")]
    public string Type { get; set; }
}