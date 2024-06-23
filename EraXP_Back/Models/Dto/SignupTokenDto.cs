using System.Text.Json.Serialization;
using EraXP_Back.Models.Domain.Enum;

namespace EraXP_Back.Models.Dto;

public record SignupTokenDto(
    [property: JsonPropertyName("ut")] UserType UserType,
    [property: JsonPropertyName("em")] string? Email,
    [property: JsonPropertyName("ed")] DateTimeOffset ExpirationDate
)
{
    public bool IsProfessor()
    {
        return UserType == UserType.Professor;
    }
    
    public bool IsStudent()
    {
        return UserType == UserType.Student;
    }
    
    public bool IsUniAdmin()
    {
        return UserType == UserType.UniAdmin;
    }
    
    public bool IsAdmin()
    {
        return UserType == UserType.Admin;
    }
    
    public bool IsUniMember()
    {
        return ((int)UserType) / 100 == 1;
    }
    
    public bool IsDepartmentMember()
    {
        int userTypeInInt = ((int)UserType);
        return userTypeInInt >= 100 && userTypeInInt < 150;
    }

    public bool IsValid(SignUpDto dto)
    {
        return (!IsUniMember() || dto.UniInfoDto != null) && (!IsDepartmentMember() || dto.UniInfoDto?.DepartmentId == null);
    }
}