using System.ComponentModel;
using ModelContextProtocol.Server;

namespace _5_DeployToAcaHttpSupport.Resources;

[McpServerResourceType]
public static class WeatherHistory
{
    [McpServerResource(Name = "WeatherHistory", Title = "Weather History Resource",MimeType = "text/plain")]
    [Description("Returns a summary of the weather from a file.")]
    public static string GetWeatherHistoryFromFile()
    {
        // This method is a placeholder for the actual implementation that would retrieve weather history data.
        // In a real application, this could involve querying a database or an file
        return "Ljubljana, 2023-10-01, Sunny, 20°C\n" +
               "Ljubljana, 2023-10-02, Cloudy, 18°C\n" +
               "Ljubljana, 2024-10-03, Rainy, 15°C\n" +
               "Ljubljana, 2024-10-04, Windy, 17°C\n" +
               "Ljubljana, 2025-10-05, Snowy, -2°C";
    }
}