using EraXP_Back.Models.Domain.Enum;
using Microsoft.AspNetCore.Identity;

namespace EraXP_Back.Models.Dto;

public record SignUpDto(
    string Username,
    string Password,
    string PasswordRepeat,
    string Email,
    UniversityInfoDto? UniInfoDto
)
{
}