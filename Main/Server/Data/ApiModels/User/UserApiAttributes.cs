using System.Text.Json.Serialization;
using Server.Data.Api;
using Server.Data.Api.Interfaces;
using Server.Data.User;

namespace Server.Data.ApiModels.User;

public class UserApiAttributes : IApiAttributes
{
    [JsonIgnore] public string Id { get; set; }

    [JsonIgnore] public string Type { get; set; }
    [JsonIgnore] public bool IsEmpty { get; set; }

    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; set; }

    [JsonPropertyName("userName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UserName { get; set; }
    
    [JsonPropertyName("rating")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Rating { get; set; }
    
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Name { get; set; }
    
    [JsonPropertyName("lastName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string LastName { get; set; }

    public UserApiAttributes(UserModel user)
    {
        Id = user.Id.ToString();
        Type = ApiTypes.User;
        Email = user.Email;
        UserName = user.UserName;
        Rating = user.Rating;
        Name = user.Name;
        LastName = user.LastName;
    }
}