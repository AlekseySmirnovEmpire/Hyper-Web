using System.Text.Json.Serialization;
using Server.Data.Api.Interfaces;

namespace Server.Data.User;

public class UserModel : IApiRelationshipModel
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public bool IsActive { get; set; }

    public string Name { get; set; }

    public string LastName { get; set; }

    public int Rating { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? TokenExpireAt { get; set; }

    [JsonIgnore] public Guid RoleId { get; set; }

    public UserRole Role { get; set; }

    public DateTime CreateAt { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? BlockedAt { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; set; }
}