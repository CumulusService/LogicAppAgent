using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace LogicAppAgent;

public class Settings
{
    private readonly IConfigurationRoot configRoot;
    private AzureOpenAISettings? _azureOpenAI;
    private LogicAppSettings? _logicApp;

    public AzureOpenAISettings AzureOpenAI => _azureOpenAI ??= GetSettings<AzureOpenAISettings>();
    public LogicAppSettings LogicApp => _logicApp ??= GetSettings<LogicAppSettings>();

    public class AzureOpenAISettings
    {
        public required string ChatModelDeployment { get; set; }
        public required string Endpoint { get; set; }
        public required string ApiKey { get; set; }
    }

    public TSettings GetSettings<TSettings>() where TSettings : class =>
        this.configRoot.GetRequiredSection(typeof(TSettings).Name).Get<TSettings>() 
            ?? throw new InvalidOperationException($"Configuration section for {typeof(TSettings).Name} is missing or invalid");

    public Settings()
    {
        this.configRoot = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .Build();
    }
}