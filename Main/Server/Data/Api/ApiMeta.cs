using System.Text.Json.Serialization;

namespace Server.Data.Api;

public class ApiMeta
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
}