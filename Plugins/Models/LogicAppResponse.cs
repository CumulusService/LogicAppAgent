using System.Text.Json.Serialization;

namespace LogicAppAgent.Models;

public class LogicAppResponse
{
    [JsonPropertyName("statusCode")]
    public string? StatusCode { get; set; }

    [JsonPropertyName("body")]
    public LogicAppResponseBody? Body { get; set; }
}

public class LogicAppResponseBody
{
    [JsonPropertyName("outputs")]
    public string? Outputs { get; set; }
}
