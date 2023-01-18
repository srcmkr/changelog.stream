using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using CLApi.Models.FreshDesk;
using CLApi.Models.Generic;
using Newtonsoft.Json;

namespace CLApi.Services;

public class FreshworksApi
{
    private readonly HttpClient _client;
    private readonly IConfiguration _config;
    
    public FreshworksApi(HttpClient client, IConfiguration config)
    {
        _client = client;
        _config = config;
    }

    private async Task<HttpResponseMessage> GetTicketsByPageAsync(string build, int page)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", 
            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_config["FD_APITOKEN"]}:X"))
        );
        var query = $"({_config["FD_FIXED"]}:'{build}' OR custom_string:'{build}') AND status:{_config["FD_CLOSEDID"]}";
        var url = $"{_config["FD_ENDPOINT"]}/api/v2/search/tickets?query=\"{UrlEncoder.Default.Encode(query)}\"&page={page}";
        return await _client.GetAsync(url);
    }

    public async Task<List<GenericTicket>> GetAllTicketsAsync(string build, string date)
    {
        var ticketList = new List<GenericTicket>();
        var page = 1;
        var lastPage = false;
        
        // https://developers.freshdesk.com/api/#filter_tickets
        // 8. The number of objects returned per page is 30 also the total count of the results will be returned along with the result
        const int maxPerPage = 30;

        // 9. To scroll through the pages add page parameter to the url. The page number starts with 1 and should not exceed 10
        while (!lastPage && page <= 10)
        {
            var ticketGetResponse = await GetTicketsByPageAsync(build, page);

            if (!ticketGetResponse.IsSuccessStatusCode)
            {
                var errorCode = ticketGetResponse.StatusCode;
                var errorMessage = await ticketGetResponse.Content.ReadAsStringAsync();
                ticketList.Add(new GenericTicket
                {
                    TicketId = errorCode.ToString(),
                    Subject = errorMessage,
                    Type = "Error",
                    SolvedInVersion = "Error",
                    SolvedInDate = date
                });
                return ticketList;
            }
        
            var ticketGetContent = await ticketGetResponse.Content.ReadAsStringAsync();
        
            var tickets = JsonConvert.DeserializeObject<FreshDeskFilterResponse>(ticketGetContent);
            if (tickets?.Tickets == null) return ticketList;
            
            foreach (var ticket in tickets.Tickets)
            {
                var caption = ticket.CustomFields.TryGetValue(_config["FD_ENTRY"], out var captOut) ? captOut : null;
                if (string.IsNullOrWhiteSpace(caption)) caption = ticket.Subject;
                var fixedVersion = ticket.CustomFields.TryGetValue(_config["FD_FIXED"], out var fvOut) ? fvOut : null;

                if (caption == null || fixedVersion == null) continue;
            
                ticketList.Add(new GenericTicket
                {
                    Subject = caption,
                    Type = ticket.Type,
                    TicketId = ticket.Id.ToString(),
                    SolvedInVersion = fixedVersion,
                    SolvedInDate = date
                });
            }
            
            if (tickets.Total <= maxPerPage * page) lastPage = true;
            page++;
        }
        
        return ticketList;
    }
}