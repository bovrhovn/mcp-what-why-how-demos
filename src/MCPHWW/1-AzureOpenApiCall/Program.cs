using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using Spectre.Console;

AnsiConsole.Write(new Markup("[blue]1 - Calling deployments from Azure OpenAI Service...[/]"));
AnsiConsole.WriteLine();
var projectEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? string.Empty;
ArgumentException.ThrowIfNullOrEmpty(projectEndpoint);
var projectKey = Environment.GetEnvironmentVariable("APIKEY") ?? string.Empty;
ArgumentException.ThrowIfNullOrEmpty(projectKey);

var deploymentName = Environment.GetEnvironmentVariable("DEPLOYMENTNAME") ?? "gpt-4o";
AnsiConsole.Write(new Markup("[blue]Project endpoint: [/]" + projectEndpoint));
AnsiConsole.WriteLine();
AnsiConsole.Write(new Markup("[blue]Deployment Name: [/]" + deploymentName));
AnsiConsole.WriteLine();

var client = new AzureOpenAIClient(new Uri(projectEndpoint),
    new AzureKeyCredential(projectKey));
var chatClient = client.GetChatClient(deploymentName);
var requestOptions = new ChatCompletionOptions
{
    MaxOutputTokenCount = 4096,
    Temperature = 1.0f,
    TopP = 1.0f
};

var table = new Table();
table.AddColumn("Who");
table.AddColumn("Response");
var question = AnsiConsole.Prompt(
    new TextPrompt<string>("What's your question?")
        .DefaultValue("What is the weather in Slovenia?"));
var messages = new List<ChatMessage>
{
    new SystemChatMessage("You are a chat assistant that helps users with their queries. " +
                          "You should provide helpful and accurate responses based on the user's input." +
                          "If you don't know the answer say 'I don't know, try again'."),
    new UserChatMessage(question)
};
table.AddRow("User", question);
var response = await chatClient.CompleteChatAsync(messages, requestOptions);
if (response.Value == null)
    throw new InvalidOperationException("No response from the chat service.");

var assistantMessage = response.Value.Content.FirstOrDefault()?.Text;
if (string.IsNullOrEmpty(assistantMessage))
    throw new InvalidOperationException("No content in the response from the chat service.");

table.AddRow("Assistant", assistantMessage);
AnsiConsole.Write(table);
AnsiConsole.WriteLine();
AnsiConsole.Write(new Markup("[ bold green]Chat completed successfully![/]"));