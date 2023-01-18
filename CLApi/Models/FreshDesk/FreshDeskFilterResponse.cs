using Newtonsoft.Json;

namespace CLApi.Models.FreshDesk;

public class FreshDeskFilterResponse
{
    [JsonProperty("results")]
    public FreshDeskTicket[] Tickets { get; set; }
    
    [JsonProperty("total")]
    public int Total { get; set; }
}