using EraXP_Back.Models.Domain.Enum;

namespace EraXP_Back.Models.Dto;

public record UserDto(
    string Username,
    string Email,
    UserType UserType,
    Guid? UniversityId,
    Guid? DepartmentId
);