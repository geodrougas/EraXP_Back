namespace EraXP_Back.Models.Database;

public record StudentUniversityInfo(
    Guid Id,
    Guid UserId,
    Guid UniversityId,
    Guid DepartmentId
) : IUserUniversityInfo
{
    Guid? IUserUniversityInfo.DepartmentId => this.DepartmentId;
}