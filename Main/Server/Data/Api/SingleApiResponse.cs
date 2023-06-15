using System.Text.Json.Serialization;
using Server.Data.Api.Interfaces;

namespace Server.Data.Api;

public class SingleApiResponse: IApiResponse
{
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ApiData? Data { get; set; }
    
    [JsonPropertyName("meta")]
    public ApiMeta Meta { get; set; }

    public SingleApiResponse(ApiData? data)
    {
        Data = data;
        Meta = new ApiMeta
        {
            Count = data == null ? 0 : 1
        };
    }
}