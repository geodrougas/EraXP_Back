namespace EraXP_Back.Models.Database;

public record ProfessorUniversityInfo(
    Guid Id,
    Guid UserId,
    Guid UniversityId
) : IUserUniversityInfo
{
    public Guid? DepartmentId => null;
}