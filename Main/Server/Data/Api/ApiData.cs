using System.Text.Json.Serialization;
using Server.Data.Api.Interfaces;

namespace Server.Data.Api;

public class ApiData
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("attributes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IApiAttributes? Attributes { get; set; }
    
    [JsonPropertyName("relationships")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IApiRelationship? Relationships { get; set; }

    public ApiData(IApiAttributes attributes, IApiRelationship? relationships = null)
    {
        Id = attributes.Id;
        Type = attributes.Type;
        if (!attributes.IsEmpty)
        {
            Attributes = attributes;
        }
        
        Relationships = relationships;
    }
}