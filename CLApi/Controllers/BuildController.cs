using System.Net.Mime;
using CLApi.Models.Generic;
using CLApi.Services;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace CLApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BuildController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public BuildController(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _config = configuration;
    }

    /// <summary>
    /// Creates a release for a given version
    /// </summary>
    /// <param name="buildId">Build ID as found in a custom field (e.g. cf_fixed_in_version)</param>
    /// <param name="token">Secret token to avoid certain "background noise" as entered in appsettings.json</param>
    /// <param name="date">Release Date of version (optional, can be overwritten)</param>
    /// <response code="200">If any closed tickets with that build number and changelog message were found, returns amount of tickets as int</response>
    /// <response code="401">If token does not fit the appsettings.json value</response>
    /// <response code="404">If no tickets with build number and changelog message were found</response>
    [HttpGet("Create/{buildId}/{token}/{date}")]
    [HttpGet("Create/{buildId}/{token}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> CreateBuild(string buildId, string token, string? date)
    {
        date ??= DateTime.Now.ToString("yyyy-MM-dd");
        
        // Check if token is valid
        if (_config["TOKEN"] != token) return Unauthorized("Invalid token");

        var api = new FreshworksApi(_httpClient, _config);
        var tickets = await api.GetAllTicketsAsync(buildId, date);
        if (!tickets.Any()) return NotFound("No tickets found");

        using (var db = new LiteDatabase("data/cl.db"))
        {
            var collection = db.GetCollection<GenericTicket>("builds");
            collection.DeleteMany(c => c.SolvedInVersion == buildId);
            foreach (var ticket in tickets)
            {
                collection.Insert(ticket);
            }
        }

        return Ok(tickets.Count);
    }
}