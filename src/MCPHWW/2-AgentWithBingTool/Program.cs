using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Spectre.Console;

AnsiConsole.Write(new Markup("[blue]2 - Calling agent with Bing tool from Azure OpenAI Service...[/]"));
AnsiConsole.WriteLine();

#region Environment Variables

var projectEndpoint = Environment.GetEnvironmentVariable("AI_FOUNDRY_PROJECT_ENDPOINT");
ArgumentException.ThrowIfNullOrEmpty(projectEndpoint);
var bingConnectionId = Environment.GetEnvironmentVariable("BING_CONNECTION_ID");
ArgumentException.ThrowIfNullOrEmpty(bingConnectionId);

var deploymentName = Environment.GetEnvironmentVariable("DEPLOYMENTNAME") ?? "gpt-4o";
AnsiConsole.Write(new Markup("[blue]Project endpoint: [/]" + projectEndpoint));
AnsiConsole.WriteLine();
AnsiConsole.Write(new Markup("[blue]Deployment Name: [/]" + deploymentName));
AnsiConsole.WriteLine();
AnsiConsole.Write(new Markup("[blue]Bing ID: [/]" + bingConnectionId));
AnsiConsole.WriteLine();

#endregion

//use CLI credentials to connect to the Azure Foundry project
var defaultAzureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    ExcludeAzureCliCredential = false,
    ExcludeEnvironmentCredential = true,
    ExcludeInteractiveBrowserCredential = true,
    ExcludeVisualStudioCodeCredential = true,
    ExcludeVisualStudioCredential = true
});

PersistentAgentsClient client = new(projectEndpoint, defaultAzureCredential);

var searchConfig = new BingGroundingSearchConfiguration(bingConnectionId)
{
    Count = 5, Freshness = "Day"
};

var bingGroundingTool = new BingGroundingToolDefinition(
    new BingGroundingSearchToolParameters([searchConfig])
);

var instructions = "Use the bing grounding tool to answer questions. " +
                   "Tell me the time of the data you are using to answer the question.";
PersistentAgent agentClient = client.Administration.CreateAgent(
    model: deploymentName,
    name: "AgentWithBingTool",
    instructions: instructions,
    tools: [bingGroundingTool]
);

var question = AnsiConsole.Prompt(
    new TextPrompt<string>("What's your question?")
        .DefaultValue("What is the weather in Slovenia?"));

PersistentAgentThread thread = client.Threads.CreateThread();
// Create message to thread
PersistentThreadMessage message = client.Messages.CreateMessage(
    thread.Id,
    MessageRole.User,
    question);
AnsiConsole.Write(new Markup("[blue]Sending messages to agents....[/]"));
AnsiConsole.WriteLine();
// Run the agent
ThreadRun run = client.Runs.CreateRun(thread, agentClient);
do
{
    Thread.Sleep(TimeSpan.FromMilliseconds(500));
    run = client.Runs.GetRun(thread.Id, run.Id);
} while (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress);

AnsiConsole.Write(new Markup("[blue]Run status[/]: " + run.Status));
// Confirm that the run completed successfully
if (run.Status != RunStatus.Completed)
{
    AnsiConsole.WriteException(new Exception("Run did not complete successfully, error: " + run.LastError?.Message));
    return;
}

Pageable<PersistentThreadMessage> messages = client.Messages.GetMessages(
    threadId: thread.Id,
    order: ListSortOrder.Ascending
);

foreach (var threadMessage in messages)
{
    AnsiConsole.Write(new Markup("[blue]" + threadMessage.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") +
                                 "[/] - [blue]Role[/]: "));
    AnsiConsole.Write(threadMessage.Role.ToString().ToUpperInvariant());
    AnsiConsole.WriteLine();
    foreach (var contentItem in threadMessage.ContentItems)
    {
        switch (contentItem)
        {
            case MessageTextContent textItem:
            {
                var response = textItem.Text;
                if (textItem.Annotations != null)
                {
                    foreach (var annotation in textItem.Annotations)
                    {
                        if (annotation is MessageTextUriCitationAnnotation urlAnnotation)
                        {
                            response = response.Replace(urlAnnotation.Text,
                                $" [{urlAnnotation.UriCitation.Title}]({urlAnnotation.UriCitation.Uri})");
                        }
                    }
                }

                AnsiConsole.Write(new Markup("[bold red]Agent response: [/]"));
                AnsiConsole.WriteLine(response);
                break;
            }
        }
        AnsiConsole.WriteLine();
    }
}

AnsiConsole.WriteLine("Done with the agent with Bing tool example.");