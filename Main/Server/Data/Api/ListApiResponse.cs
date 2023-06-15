using System.Text.Json.Serialization;
using Server.Data.Api.Interfaces;

namespace Server.Data.Api;

public class ListApiResponse : IApiResponse
{
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ApiData>? DataList { get; set; }
    
    [JsonPropertyName("meta")]
    public ApiMeta Meta { get; set; }

    public ListApiResponse(List<ApiData>? dataList)
    {
        DataList = dataList;
        Meta = new ApiMeta
        {
            Count = dataList?.Count ?? 0
        };
    }
}