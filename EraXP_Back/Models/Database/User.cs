namespace EraXP_Back.Models.Database;

public record User(
    Guid Id,
    string Username,
    string Base64HashedPassword,
    string NormalisedUsername,
    string Email,
    string NormalisedEmail,
    Guid UniversityId,
    Guid DepartmentId,
    Guid SecurityStamp,
    Guid ConcurrencyStamp,
    bool RolesLoaded,
    List<Role>? UserRoles 
);