using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

// Set Null Value Handling to ignore (no more "xxx: null" in output) 
builder.Services.AddControllers().AddNewtonsoftJson(o => { o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore; });

// Add OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo{ Title = "changelog.stream", Version = "v1" });
});
builder.Services.AddHttpClient();

// Add Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        theme: AnsiConsoleTheme.Code,
        restrictedToMinimumLevel: LogEventLevel.Information, 
        outputTemplate: "    [{Timestamp:dd.MM. HH:mm:ss}] [{Level:u3}] {Message}{NewLine}{Exception}")
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

var app = builder.Build();

// Show configuration in console on startup
Log.Information(@"
       _                            _                   _                            
      | |                          | |                 | |                           
   ___| |__   __ _ _ __   __ _  ___| | ___   __ _   ___| |_ _ __ ___  __ _ _ __ ___  
  / __| '_ \ / _` | '_ \ / _` |/ _ \ |/ _ \ / _` | / __| __| '__/ _ \/ _` | '_ ` _ \ 
 | (__| | | | (_| | | | | (_| |  __/ | (_) | (_| |_\__ \ |_| | |  __/ (_| | | | | | |
  \___|_| |_|\__,_|_| |_|\__, |\___|_|\___/ \__, (_)___/\__|_|  \___|\__,_|_| |_| |_|
                          __/ |              __/ | github.com/srcmkr/changelog.stream                                   
                         |___/              |___/  Licensed under the MIT License
                                  
");
Log.Information("FreshDesk Endpoint: {FreshDeskEndpoint}", builder.Configuration["FD_ENDPOINT"]);

// Validate configuration (there must be a better solution for this but I don't know it)
if (string.IsNullOrWhiteSpace(builder.Configuration["FD_ENDPOINT"])) throw new Exception("FD_ENDPOINT is not set. Aborting");
if (string.IsNullOrWhiteSpace(builder.Configuration["FD_FIXED"])) throw new Exception("FD_FIXED is not set. Aborting");
if (string.IsNullOrWhiteSpace(builder.Configuration["FD_APITOKEN"])) throw new Exception("FD_APITOKEN is not set. Aborting");
if (string.IsNullOrWhiteSpace(builder.Configuration["TOKEN"])) throw new Exception("TOKEN is not set. Aborting");
if (builder.Configuration["FD_CLOSEDID"] == null) throw new Exception("FD_CLOSEDID is not set. Aborting");

// Enable Swagger for OpenAPI
app.UseSwagger();
app.UseSwaggerUI();

// Go
app.MapControllers();
app.Run();