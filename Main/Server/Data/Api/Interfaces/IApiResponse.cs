using System.Text.Json.Serialization;

namespace Server.Data.Api.Interfaces;

public interface IApiResponse
{
    [JsonPropertyName("meta")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ApiMeta? Meta { get; set; }
}