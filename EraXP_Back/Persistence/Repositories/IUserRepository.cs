using EraXP_Back.Models.Database;

namespace EraXP_Back.Persistence.Repositories;

public interface IUserRepository
{
    Task<User?> Get(Guid? id = null, string? username = null, Guid? securityToken = null);
    Task<ProfessorUniversityInfo?> GetProfessorsUniversityInfo(Guid userId);
    Task<StudentUniversityInfo?> GetStudentUniversityInfo(Guid userId);
}