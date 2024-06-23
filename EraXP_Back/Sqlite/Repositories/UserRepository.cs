using EraXP_Back.Models.Database;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;

namespace EraXP_Back.Sqlite.Repositories;

public class UserRepository(IDbExec dbExec) : IUserRepository
{
    private readonly IDbExec _dbExec = dbExec;

    public Task<User?> Get(Guid? id = null, string? username = null, Guid? securityToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<ProfessorUniversityInfo?> GetProfessorsUniversityInfo(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<StudentUniversityInfo?> GetStudentUniversityInfo(Guid userId)
    {
        throw new NotImplementedException();
    }
}