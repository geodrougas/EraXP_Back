using EraXP_Back.Models;
using EraXP_Back.Models.Database;

namespace EraXP_Back.Repositories;

public interface IUserRepository
{
    Task<User?> GetUser(string username);
    Task<User?> GetUser(Guid id, Guid? securityToken = null);
    Task<User?> GetUserWithRoles(Guid id, Guid? securityToken = null);
    Task<bool> UserExistsWithFollowing(string userDtoUsername, string userDtoEmail);
    Task Save(User user);
}