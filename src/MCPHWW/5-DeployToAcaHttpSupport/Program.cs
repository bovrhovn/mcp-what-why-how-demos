
using _5_DeployToAcaHttpSupport.Resources;
using ModelContextProtocol.Server;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole(consoleLogOptions => consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace);
// var resource = McpServerResource.Create(
//     typeof(WeatherHistory).GetMethod(nameof(WeatherHistory.GetWeatherHistoryFromFile))!,
//     new WeatherHistory()
// );
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithHttpTransport(o => o.Stateless = true)
    .WithResourcesFromAssembly(typeof(WeatherHistory).Assembly)
    .WithToolsFromAssembly();

builder.Services.AddHttpClient();

var app = builder.Build();

app.MapMcp("/mcp");
app.Run();