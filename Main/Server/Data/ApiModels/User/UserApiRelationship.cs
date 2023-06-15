using System.Text.Json.Serialization;
using Server.Data.Api.Interfaces;

namespace Server.Data.ApiModels.User;

public class UserApiRelationship : IApiRelationship
{
    [JsonPropertyName("userRole")]
    public IApiResponse Object { get; set; }

    public UserApiRelationship(IApiResponse userRoles)
    {
        Object = userRoles;
    }
}