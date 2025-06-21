using ModelContextProtocol.Client;
using Spectre.Console;

AnsiConsole.Write(new Markup("[blue]3 - Creating client and running server...[/]"));
AnsiConsole.WriteLine();
var pathToWeatherServerTool= Environment.GetEnvironmentVariable("SIMPLE_WEATHER_SERVER_PATH"); 
ArgumentException.ThrowIfNullOrEmpty(pathToWeatherServerTool);

var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "WeatherClient",
    Command = "dotnet",
    Arguments = ["run", "--project", pathToWeatherServerTool]
});

var client = await McpClientFactory.CreateAsync(clientTransport);
var tools = await client.ListToolsAsync();

AnsiConsole.Write(new Markup("[blue]Available tools:[/]"));
AnsiConsole.WriteLine();
foreach (var tool in tools)
{
    var toolFormat = $"{tool.Name} - {tool.Description}";
    AnsiConsole.Write(new Markup("[bold]" + toolFormat + "[/]"));
    AnsiConsole.WriteLine();
}

var result = await client.CallToolAsync(
    "simpleWeatherTool",
    new Dictionary<string, object?> { ["location"] = "Ljubljana" }
);

AnsiConsole.Write(new Markup("[blue]Simple weather result:[/]"));
AnsiConsole.WriteLine();
AnsiConsole.WriteLine(result.Content[0].Text ?? "No data returned from the server.");
result = await client.CallToolAsync(
    "objectWeatherTool",
    new Dictionary<string, object?> { ["location"] = "Ljubljana" }
);
AnsiConsole.Write(new Markup("[blue]Object Weather result:[/]"));
AnsiConsole.WriteLine();
AnsiConsole.WriteLine(result.Content[0].Text ?? "No data returned from the server.");

AnsiConsole.Write(new Markup("[bold green]Client call completed successfully![/]"));