using EraXP_Back.Models.Domain.Enum;

namespace EraXP_Back.Models.Domain;

public record Authority(
    string AuthorizationScheme,
    string Id,
    string Key,
    EAuthorityType Type,
    string[] Roles
);
