using McpToolkit.Server;

var server = new McpServer
{
    Name = "Simple Weather MCP Server",
    Version = "1.0.0"
};

server.Tools.Add("simpleWeatherTool", "Gets one line for current weather for a location", async (string location) =>
{
    // Call weather API or service to get weather data
    var weatherData = await GetWeatherDataAsync(location);
    // Return the weather data as a response
    var data = $"Weather in {location}: {weatherData.Temperature}°C, {weatherData.Conditions}";
    return data;
});
server.Tools.Add("objectWeatherTool", "Gets object data for current weather for a location", async (string location) =>
{
    // Call weather API or service to get weather data
    var weatherData = await GetWeatherDataAsync(location);
    return weatherData;
});

// Connect the server using stdio transport
var transport = new StdioServerTransport();
await server.ConnectAsync(transport);

Console.WriteLine("Weather MCP Server started");

// Keep the server running until process is terminated
await Task.Delay(-1);

async Task<WeatherData> GetWeatherDataAsync(string location)
{
    // This would normally call a weather API
    // Simplified for demonstration
    await Task.Delay(100); // Simulate API call
    return new WeatherData
    {
        Temperature = 24.2,
        Conditions = "Sunny",
        Location = location
    };
}