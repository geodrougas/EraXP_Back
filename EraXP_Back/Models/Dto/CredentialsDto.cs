using EraXP_Back.Models.Domain.Enum;

namespace EraXP_Back.Models.Dto;

public record CredentialsDto(
    string Username,
    string Password,
    EAuthorizationMethod AuthorizationMethod
);