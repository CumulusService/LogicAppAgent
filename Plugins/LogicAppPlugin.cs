using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LogicAppAgent.Models;
using Microsoft.SemanticKernel;

namespace LogicAppAgent.Plugins;

public class LogicAppPlugin
{
    private readonly HttpClient _httpClient;
    private readonly LogicAppSettings _settings;

    public LogicAppPlugin(LogicAppSettings settings)
    {
        _settings = settings;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
       /* if (!string.IsNullOrEmpty(settings.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("x-functions-key", settings.ApiKey);
        }*/
    }

    [KernelFunction, Description("Triggers a Logic App workflow and returns the response")]
    public async Task<string> TriggerWorkflowAsync(
        [Description("Parameters to pass to the Logic App workflow")] string parameters)
    {
        var triggerUrl = _settings.LogicAppUrl;
        
        var content = new StringContent(parameters, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(triggerUrl, content);
        
        response.EnsureSuccessStatusCode();
        
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var logicAppResponse = JsonSerializer.Deserialize<LogicAppResponseBody>(jsonResponse);
        
        if (logicAppResponse == null || logicAppResponse.Outputs == null)
        {
            throw new InvalidOperationException("The response from the Logic App is null or invalid.");
        }
        
        return JsonSerializer.Serialize(logicAppResponse.Outputs);
    }
}