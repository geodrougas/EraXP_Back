using EraXP_Back.Models.Domain.Enum;

namespace EraXP_Back.Models.Database;

public record User(
    Guid Id,
    string Username,
    string Base64HashedPassword,
    string NormalisedUsername,
    string Email,
    string NormalisedEmail,
    UserType UserType,
    Guid SecurityStamp,
    Guid ConcurrencyStamp
);