using EraXP_Back.Models.Database;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.Repositories;

namespace EraXP_Back.PostgresQL.Repositories;

public class UserRepository(IDbExec dbExec) : IUserRepository
{
    private readonly IDbExec _dbExec = dbExec;

    public Task<User?> Get(Guid? id = null, string? username = null, Guid? securityToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserWithRoles(Guid? id = null, string? username = null, Guid? securityToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckIfUserExistsOr(Guid? id = null, string? username = null, string? email = null)
    {
        throw new NotImplementedException();
    }
}