using System.Text.Json.Serialization;
using Server.Data.Api.Interfaces;

namespace Server.Data.ApiModels.User;

public class UserRoleApiRelationship : IApiRelationship
{
    [JsonPropertyName("user")]
    public IApiResponse Object { get; set; }

    public UserRoleApiRelationship(IApiResponse user)
    {
        Object = user;
    }
}