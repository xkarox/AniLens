using System.Runtime.CompilerServices;

namespace AniLens.Shared;

public enum UserRole
{
    None,
    User,
    Moderator,
    Admin
}

public static class UserRoleExtensions
{
    public static string ToString(this UserRole role)
    {
        return role switch
        {
            UserRole.None => "None",
            UserRole.User => "User",
            UserRole.Moderator => "Moderator",
            UserRole.Admin => "Admin",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
}


