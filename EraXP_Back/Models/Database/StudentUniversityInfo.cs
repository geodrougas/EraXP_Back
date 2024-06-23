namespace EraXP_Back.Models.Database;

public record StudentUniversityInfo(
    Guid Id,
    Guid UserId,
    Guid UniversityId,
    Guid DepartmentId
) : UserUniversityInfo
{
    Guid? UserUniversityInfo.DepartmentId => this.DepartmentId;
}