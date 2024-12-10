using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text;
using LogicAppAgent.Models;

namespace LogicAppAgent.Agents;

public class LogicAppCompletionAgent
{
    private readonly ChatCompletionAgent _agent;

    public LogicAppCompletionAgent(Kernel kernel)
    {
        _agent = new ChatCompletionAgent
        {
            Name = "LogicAppAgent",
            Instructions = @"You are an agent designed to interact with Azure Logic Apps workflows.
                               You can trigger workflows and process their responses.
                               When a user asks to execute a workflow, use the LogicAppPlugin to trigger it.
                               Format the parameters as valid JSON before sending them to the workflow.
                               After receiving the response, explain the results in a clear and concise way.",
            Kernel = kernel,
            Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    ResponseFormat = typeof(LogicAppResponse)
                })
        };
    }

    public async Task<string> InvokeAsync(string userInput)
    {
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage(userInput);

        var response = new StringBuilder();
        await foreach (var message in _agent.InvokeAsync(chatHistory))
        {
            response.Append(message.Content);
        }

        return response.ToString();
    }
}

public class AzureOpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string ChatModelDeployment { get; set; } = string.Empty;
}
