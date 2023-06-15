using System.Text.Json.Serialization;
using Server.Data.Api.Interfaces;

namespace Server.Data.User;

public class UserRole : IApiRelationshipModel
{
    [JsonIgnore]
    public Guid Id { get; set; }
    
    public string RoleName { get; set; }

    [JsonIgnore]
    public ICollection<UserModel> Users { get; set; }
}