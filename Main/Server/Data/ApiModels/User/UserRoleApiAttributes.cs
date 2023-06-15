using System.Text.Json.Serialization;
using Server.Data.Api;
using Server.Data.Api.Interfaces;
using Server.Data.User;

namespace Server.Data.ApiModels.User;

public class UserRoleApiAttributes: IApiAttributes
{
    [JsonIgnore]
    public string Id { get; set; }
    
    [JsonIgnore]
    public string Type { get; set; }
    
    [JsonIgnore]
    public bool IsEmpty { get; set; }
    
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    public UserRoleApiAttributes(UserRole userRole)
    {
        Id = userRole.Id.ToString();
        Type = ApiTypes.UserRole;
        Name = userRole.RoleName;
    }
}