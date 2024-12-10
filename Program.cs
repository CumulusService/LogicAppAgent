using LogicAppAgent.Agents;
using LogicAppAgent.Plugins;
using Microsoft.SemanticKernel;
namespace LogicAppAgent;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Load configuration from environment variables or user secrets
        var settings = new Settings();

        Console.WriteLine("Initialize plugins...");
        var logicAppSettings = settings.GetSettings<LogicAppSettings>();
        var logicAppPlugin = new LogicAppPlugin(logicAppSettings);

        Console.WriteLine("Creating kernel...");
        var builder = Kernel.CreateBuilder();
        /*builder.AddAzureOpenAIChatCompletion(
            settings.AzureOpenAI.ChatModelDeployment,
            settings.AzureOpenAI.Endpoint,
            settings.AzureOpenAI.ApiKey);*/
        builder.AddOpenAIChatCompletion(
            modelId:settings.OpenAI.ChatModel,
            apiKey:settings.OpenAI.ApiKey);     

        builder.Plugins.AddFromObject(logicAppPlugin, "LogicAppPlugin");
        var kernel = builder.Build();

        Console.WriteLine("Initializing agent...");
        var agent = new LogicAppCompletionAgent(kernel);

        Console.WriteLine("Ready! Type 'exit' to quit.");
        bool isComplete = false;
        do
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                isComplete = true;
                break;
            }

            try
            {
                var response = await agent.InvokeAsync(input);
                Console.WriteLine();
                Console.WriteLine($"Agent: {response}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        } while (!isComplete);
    }
}
