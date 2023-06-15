using Server.Data;
using Server.Data.User;

namespace Server.Services;

public class RoleManager
{
    private readonly ApplicationDbContext _dbContext;
    private static ICollection<UserRole>? _roles;

    public IEnumerable<UserRole> AllRoles
    {
        get
        {
            _roles ??= _dbContext.Roles.ToList();
            return _roles;
        }
    }

    public RoleManager(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public UserRole GetMemberRole() => AllRoles.First(r => r.RoleName == Roles.Member);
}