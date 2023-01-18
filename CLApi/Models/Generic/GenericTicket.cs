using LiteDB;
using Newtonsoft.Json;

namespace CLApi.Models.Generic;

public class GenericTicket
{
    // Internal ticket ID used by DB
    [JsonIgnore][BsonId]
    public int Id { get; set; }
    
    // What exactly is the delta in that release? (e.g. "Added a new spinner loading component")
    public string Subject { get; set; }
    
    // Solved in which version? (e.g. 1.0.0-rc1 or 4.3.2)
    public string SolvedInVersion { get; set; }
    public string SolvedInDate { get; set; }

    // Ticket Type (e.g. Change, Bugfix, Feature, etc.)
    public string Type { get; set; }
    
    // Optional: Ticket ID (e.g. #1234)
    public string TicketId { get; set; }
}