# Model-Context-Protocol: What, how and why

The Model Context Protocol (MCP) is an open, standardized interface that allows Large Language Models (LLMs) to interact
seamlessly with external tools, APIs, and data sources. It provides a consistent architecture to enhance AI model
functionality beyond their training data, enabling smarter, scalable, and more responsive AI systems.
MCP is an open protocol that standardizes how applications provide context to LLMs. Think of MCP like a USB-C port for
AI applications.

![MCP overview - picture from https://modelcontextprotocol.io/introduction](https://webeudatastorage.blob.core.windows.net/web/mcp-overview.png)

**Figure 1**: Picture from [modelcontextprotocol.io](https://modelcontextprotocol.io/introduction)

## What is the problem MCP tries to solve

- Custom code per tool-model pair
- Non-standard APIs for each vendor
- Frequent breaks due to updates
- Poor scalability with more tools

Let us take a look at the [Model Context Protocol](https://modelcontextprotocol.io/introduction) and how it can be used
in practice.

## Prerequisites

1. [PowerShell](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-7.2)
   installed - we do recommend an editor like [Visual Studio Code](https://code.visualstudio.com) to be able to write
   scripts and work with code.
2. git installed - instructions step by step [here](https://docs.github.com/en/get-started/quickstart/set-up-git)
3. [.NET](https://dot.net) installed to run the application if you want to run it
4. an editor (besides notepad) to see and work with code, yaml, scripts and more (for
   example [Visual Studio Code](https://code.visualstudio.com) or [Visual Studio](https://visualstudio.microsoft.com/)
   or [Jetbrains Rider](https://jetbrains.com/rider))
5. [OPTIONAL] GitHub CLI installed to work with GitHub - [how to install](https://cli.github.com/manual/installation)
6. [OPTIONAL] [Github GUI App](https://desktop.github.com/) for managing changes and work
   on [forked](https://docs.github.com/en/get-started/quickstart/fork-a-repo) repo
7. [OPTIONAL] [Windows Terminal](https://learn.microsoft.com/en-us/windows/terminal/install)

To work with the demos in this repository, you will need to have access to an Azure AI Foundry. You can get access
by applying by following
this [getting started](https://learn.microsoft.com/en-us/azure/ai-foundry/quickstarts/get-started-code?tabs=azure-ai-foundry&pivots=fdp-project)
tutorial and following the instructions to set up your
Azure environment. Once you have access, you will need to set up the environment variables for the demos to work.

## Demo structure

1. [**1-AzureOpenAPICall**](src/MCPHWW/1-AzureOpenApiCall) - This demo shows how to call an Azure OpenAI model using the
   Model Context Protocol. It demonstrates
   how to set up the environment variables and make a simple call to the model.

## Getting Started

To get started with the Model Context Protocol, you can follow these steps:

- Clone the repository:

``` powershell

git clone https://github.com/bovrhovn/mcp-what-why-how-demos.git
cd mcp-what-why-how-demos\src\MCPHWW

```

- set up the environment variables for the demo. You can do this by running the following PowerShell commands in your
  terminal:

``` powershell

Set-EnvironmentVariable -Name "AI_FOUNDRY_PROJECT_ENDPOINT" -Value "<your-endpoint-to-the-azure-foundry>" -Scope Process
Set-EnvironmentVariable -Name "AZURE_OPENAI_ENDPOINT" -Value "<your-endpoint-to-the-deployed-model>" -Scope Process
Set-EnvironmentVariable -Name "DEPLOYMENTNAME" -Value "<your-deploymentname>" -Scope Process
Set-EnvironmentVariable -Name "APIKEY" -Value "<your-api-key-from-model-deployed>" -Scope Process

```

- Run the demos (cd into which demo you want to run and then execute the following command):

``` powershell

dotnet run

``` 

# Additional Resources

- [Model Context Protocol Documentation](https://modelcontextprotocol.io/introduction)
- [MCP for beginners](https://github.com/microsoft/mcp-for-beginners)
- [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)

# Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft trademarks
or logos is subject to and must
follow [Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks?oneroute=true).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft
sponsorship. Any use of third-party trademarks or logos are subject to those third-party's policies.