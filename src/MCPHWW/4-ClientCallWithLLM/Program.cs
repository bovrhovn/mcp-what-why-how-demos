using System.Text.Json;
using Azure.AI.Inference;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using ModelContextProtocol.Client;
using Spectre.Console;

AnsiConsole.Write(new Markup("[blue]4 - Calling MCP server from client with LLM...[/]"));
AnsiConsole.WriteLine();
var pathToWeatherServerTool= Environment.GetEnvironmentVariable("SIMPLE_WEATHER_SERVER_PATH"); 
ArgumentException.ThrowIfNullOrEmpty(pathToWeatherServerTool);

var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "WeatherClient",
    Command = "dotnet",
    Arguments = ["run", "--project", pathToWeatherServerTool]
});

await using var mcpClient = await McpClientFactory.CreateAsync(clientTransport);


var defaultAzureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    ExcludeAzureCliCredential = false,
    ExcludeEnvironmentCredential = true,
    ExcludeInteractiveBrowserCredential = true,
    ExcludeVisualStudioCodeCredential = true,
    ExcludeVisualStudioCredential = true
});
var projectEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? string.Empty;
ArgumentException.ThrowIfNullOrEmpty(projectEndpoint);

var deploymentName = Environment.GetEnvironmentVariable("DEPLOYMENTNAME") ?? "gpt-4o";
var modelName = Environment.GetEnvironmentVariable("MODELNAME") ?? "gpt-4o";
AnsiConsole.Write(new Markup("[blue]Project endpoint: [/]" + projectEndpoint));
AnsiConsole.WriteLine();
AnsiConsole.Write(new Markup("[blue]Deployment Name: [/]" + deploymentName));
AnsiConsole.WriteLine();

var clientOptions = new AzureAIInferenceClientOptions();
var tokenPolicy = new BearerTokenAuthenticationPolicy(defaultAzureCredential,
    ["https://cognitiveservices.azure.com/.default"]);
clientOptions.AddPolicy(tokenPolicy, HttpPipelinePosition.PerRetry);
var chatHistory = new List<ChatRequestMessage>
{
    new ChatRequestSystemMessage("You are a helpful assistant that knows about AI")
};
var llmClient = new ChatCompletionsClient(
    new Uri(projectEndpoint),
    defaultAzureCredential,
    clientOptions
);

var tools = await GetMcpTools();
for (var currentIndex = 0; currentIndex < tools.Count; currentIndex++)
{
    var tool = tools[currentIndex];
    var toolOutputFormat = $"{currentIndex}: {tool}";
    AnsiConsole.Write(new Markup("[blue]MCP Tools def:[/] " + toolOutputFormat));
    AnsiConsole.WriteLine();
}

// 2. Define the chat history and the user message
var question = AnsiConsole.Prompt(
    new TextPrompt<string>("What's your question?")
        .DefaultValue("What is the weather in Slovenia?"));
chatHistory.Add(new ChatRequestUserMessage(question));

var options = new ChatCompletionsOptions(chatHistory)
{
    Model = modelName,
    Tools = { tools[0] }
};

//call the model
ChatCompletions? response = await llmClient.CompleteAsync(options);
var content = response.Content;
//tool information
var calls = response.ToolCalls.FirstOrDefault();
for (int currentCallIndex = 0; currentCallIndex < response.ToolCalls.Count; currentCallIndex++)
{
    var call = response.ToolCalls[currentCallIndex];
    AnsiConsole.WriteLine($"Tool call {currentCallIndex}: {call.Name} with arguments {call.Arguments}");
    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(call.Arguments);
    var result = await mcpClient.CallToolAsync(
        call.Name,
        dict!,
        cancellationToken: CancellationToken.None
    );

    AnsiConsole.WriteLine(result.Content.First(c => c.Type == "text").Text ?? string.Empty);
}

// Print the generic response
AnsiConsole.WriteLine($"Assistant response: {content}");

ChatCompletionsToolDefinition ConvertFrom(string name, string description, JsonElement jsonElement)
{ 
    // convert the tool to a function definition
    var functionDefinition = new FunctionDefinition(name)
    {
        Description = description,
        Parameters = BinaryData.FromObjectAsJson(new
            {
                Type = "object",
                Properties = jsonElement
            },
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
    };

    // create a tool definition
    var toolDefinition = new ChatCompletionsToolDefinition(functionDefinition);
    return toolDefinition;
}

async Task<List<ChatCompletionsToolDefinition>> GetMcpTools()
{
    Console.WriteLine("Listing tools");
    var tools = await mcpClient.ListToolsAsync();

    List<ChatCompletionsToolDefinition> toolDefinitions = [];

    foreach (var tool in tools)
    {
        Console.WriteLine($"Connected to server with tools: {tool.Name}");
        Console.WriteLine($"Tool description: {tool.Description}");
        Console.WriteLine($"Tool parameters: {tool.JsonSchema}");

        tool.JsonSchema.TryGetProperty("properties", out var propertiesElement);

        var def = ConvertFrom(tool.Name, tool.Description, propertiesElement);
        Console.WriteLine($"Tool definition: {def}");
        toolDefinitions.Add(def);

        Console.WriteLine($"Properties: {propertiesElement}");        
    }

    return toolDefinitions;
}

AnsiConsole.Write(new Markup("[bold green]Client call completed successfully![/]"));