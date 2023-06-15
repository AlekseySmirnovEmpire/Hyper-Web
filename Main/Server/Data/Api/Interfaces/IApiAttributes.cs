using System.Text.Json.Serialization;

namespace Server.Data.Api.Interfaces;

public interface IApiAttributes
{
    [JsonIgnore]
    public string Id { get; set; }
    
    [JsonIgnore]
    public string Type { get; set; }
    
    [JsonIgnore]
    public bool IsEmpty { get; set; }
}