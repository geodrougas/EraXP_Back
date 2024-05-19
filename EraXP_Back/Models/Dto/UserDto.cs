namespace EraXP_Back.Models.Dto;

public record UserDto (
    string Username,
    string Password,
    string PasswordRepeat,
    string Email,
    Guid UniversityId,
    Guid DepartmentId
);