namespace EraXP_Back.Models.Database;

public interface IUserUniversityInfo
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid UniversityId { get; }
    public Guid? DepartmentId { get; }
}