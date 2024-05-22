using EraXP_Back.Models.Database;

namespace EraXP_Back.Persistence.Repositories;

public interface IUserRepository
{
    Task<User?> Get(Guid? id = null, string? username = null, Guid? securityToken = null);
    Task<User?> GetUserWithRoles(Guid? id = null, string? username = null, Guid? securityToken = null);
    Task<bool> CheckIfUserExistsOr(Guid? id = null, string? username = null, string? email = null);
}