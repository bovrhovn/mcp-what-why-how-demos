using System.ComponentModel;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace _6_ServerWithAnotherMCP.Tools;

[McpServerToolType, Description("A tool that echoes a message back to the client with calling another MCP.")]
public static class EchoTool
{
    [McpServerTool(Name = "echo", Title = "Echo Tool")]
    [Description("Echoes the message back to the client.")]
    public static async Task<string> Echo(string message)
    {
        var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = "RemoteMCP",
            Command = "npx",
            Arguments = ["-y", "@modelcontextprotocol/server-everything"]
        });

        var client = await McpClientFactory.CreateAsync(clientTransport);
        var result = await client.CallToolAsync("echo", new Dictionary<string, object?>
        {
            ["message"] = message
        });

        var dataReturned = result.Content.FirstOrDefault(c => c.Type == "text");
        var returnedMessage = "No echoing back message possible. Check logs for more details.";
        if (dataReturned is TextContentBlock textContent && !string.IsNullOrEmpty(textContent.Text)) 
            returnedMessage = textContent.Text;

        return returnedMessage;
    }
}