using System.Text.Json.Serialization;

namespace Server.Data.Api;

public class ApiError
{
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Status { get; set; }

    [JsonPropertyName("title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Title { get; set; }

    public ApiError(int status, string title)
    {
        Status = status;
        Title = title;
    }
}