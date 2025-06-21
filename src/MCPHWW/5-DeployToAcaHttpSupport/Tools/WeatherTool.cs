using System.ComponentModel;
using ModelContextProtocol.Server;

namespace _5_DeployToAcaHttpSupport.Tools;

[McpServerToolType]
public class WeatherTool
{
    [McpServerTool(Name = "weather", Title = "Weather tool")]
    [Description("Returns the current weather in a given city.")]
    public static string Echo(string location)
    {
        var randomTemperature = new Random().Next(-10, 35);
        return
            $"The current weather in {location} is sunny with a temperature of {randomTemperature}°C. " +
            $"This is a mock response for demonstration purposes.";
    }
}