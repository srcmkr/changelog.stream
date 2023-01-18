using System.Net.Mime;
using System.Text;
using CLApi.Models.Generic;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace CLApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ExportController : ControllerBase
{
    private readonly IConfiguration _configuration;
    
    public ExportController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet("json")]
    public IActionResult GetJson()
    {
        List<GenericTicket> tickets;
        using (var db = new LiteDatabase("data/cl.db"))
        {
            tickets = db.GetCollection<GenericTicket>("builds").FindAll().ToList();
        }
        
        if (tickets.Any()) return Ok(tickets);
        return NotFound("No history found");
    }

    [HttpGet("html")]
    public IActionResult GetHtml()
    {
        var sb = new StringBuilder();

        var ticketBaseUrl = _configuration["FD_TICKETURL"];
        var hasTicketUrl = !string.IsNullOrEmpty(ticketBaseUrl);
        
        sb.AppendLine($"<html><head><title>{_configuration["NAME"]}</title><style>");
        // read css file from data/design.css and add it to the sb
        var css = System.IO.File.ReadAllText("data/design.css");
        sb.AppendLine($"{css}");
        sb.AppendLine($"</style></head><body><h1>{_configuration["NAME"]}</h1>");

        using (var db = new LiteDatabase("data/cl.db"))
        {
            var tickets = db.GetCollection<GenericTicket>("builds").FindAll();
            if (tickets != null)
            {
                var groupedTickets = tickets.GroupBy(x => x.SolvedInVersion)
                    .OrderByDescending(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.GroupBy(y => y.Type));
                
                foreach (var group in groupedTickets)
                {
                    var firstTicket = group.Value.First().First();
                    sb.AppendLine("<div class=\"versionsection\">");
                    sb.AppendLine("<h2>");
                    sb.AppendLine("<span class='version'>Version " + group.Key + "</span>");
                    sb.AppendLine("<span class='date'>" + firstTicket.SolvedInDate + "</span>");
                    sb.AppendLine("</h2>");

                    foreach (var subGroup in group.Value)
                    {
                        sb.AppendLine("<table class='table'>");
                        foreach (var ticket in subGroup)
                        {
                            sb.AppendLine("<tr>");
                            sb.AppendLine("<td class='tr-type'><span class='type " + subGroup.Key.ToLower() + "'>" + subGroup.Key + "</span></td>");
                            var ticketUrl = hasTicketUrl ? $"<a href=\"{ticketBaseUrl}/{ticket.TicketId}\" target=\"_blank\">#{ticket.TicketId}</a>" : $"#{ticket.TicketId}";
                            sb.AppendLine($"<td class='tr-subjects'>{ticket.Subject} ({ticketUrl})</td>");
                            sb.AppendLine("</tr>");
                        }
                        sb.AppendLine("</table>");
                    }
                    sb.AppendLine("</div>");
                }
                
            }
        }

        sb.AppendLine("</body></html>");

        return Ok(sb.ToString());
    }
    
    [HttpGet("markdown")]
    public IActionResult GetMarkdown()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"# {_configuration["NAME"]}");
        sb.AppendLine("");

        using (var db = new LiteDatabase("data/cl.db"))
        {
            var tickets = db.GetCollection<GenericTicket>("builds").FindAll();
            if (tickets != null)
            {
                var groupedTickets = tickets.GroupBy(x => x.SolvedInVersion)
                    .OrderByDescending(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.GroupBy(y => y.Type));
                
                foreach(var group in groupedTickets)
                {
                    var firstTicket = group.Value.First().First();
                    sb.AppendLine($"## [{group.Key}] - {firstTicket.SolvedInDate}");
                    sb.AppendLine("");
                    foreach(var subGroup in group.Value)
                    {
                        sb.AppendLine($"### " + subGroup.Key);
                        foreach(var ticket in subGroup)
                        {
                            sb.AppendLine($"- {ticket.Subject} (#{ticket.TicketId})");
                        }

                        sb.AppendLine("");
                    }
                    sb.AppendLine("");
                }
            }
        }

        return Ok(sb.ToString());
    }
}