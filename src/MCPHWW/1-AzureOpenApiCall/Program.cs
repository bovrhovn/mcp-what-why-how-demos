using System.ClientModel;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using Spectre.Console;

AnsiConsole.Write(new Markup("[blue]1 - Calling deployments from Azure OpenAI Service...[/blue]"));
AnsiConsole.WriteLine();
var projectEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? string.Empty;
var projectKey = Environment.GetEnvironmentVariable("APIKEY") ?? string.Empty;
ArgumentException.ThrowIfNullOrEmpty(projectEndpoint);
ArgumentException.ThrowIfNullOrEmpty(projectKey);

var deploymentName = Environment.GetEnvironmentVariable("DEPLOYMENTNAME") ?? "gpt-4o";
AnsiConsole.Write("[blue]Project endpoint: [/blue]" + projectEndpoint);
AnsiConsole.WriteLine();
AnsiConsole.Write("[blue]Deployment Name: [/blue]" + deploymentName);
AnsiConsole.WriteLine();

var client = new AzureOpenAIClient(new Uri(projectEndpoint),
    new ApiKeyCredential(projectKey));
var chatClient = client.GetChatClient(deploymentName);
var requestOptions = new ChatCompletionOptions
{
    MaxOutputTokenCount = 4096,
    Temperature = 1.0f,
    TopP = 1.0f
};
var messages = new List<ChatMessage>
{
    new SystemChatMessage("You are a chat assistant that helps users with their queries. " +
                          "You should provide helpful and accurate responses based on the user's input." +
                          "If you don't know the answer say 'I don't know, try again'."),
    new UserChatMessage("What is the capital of Greece?")
};
var response = await chatClient.CompleteChatAsync(messages, requestOptions);
if (response.Value == null)
    throw new InvalidOperationException("No response from the chat service.");

var assistantMessage = response.Value.Content.FirstOrDefault()?.Text;
if (string.IsNullOrEmpty(assistantMessage))
    throw new InvalidOperationException("No content in the response from the chat service.");
Console.WriteLine("Assistant: " + assistantMessage);