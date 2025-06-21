using System.ComponentModel;
using ModelContextProtocol.Server;

namespace _5_DeployToAcaHttpSupport.Tools;

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool(Name = "echo", Title = "Echo Tool")]
    [Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"hello from .NET: {message}";
}
