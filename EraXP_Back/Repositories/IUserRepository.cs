using EraXP_Back.Models;
using EraXP_Back.Models.Database;

namespace EraXP_Back.Repositories;

public interface IUserRepository
{
    Task<User?> GetUser(string username, Guid? securityToken = null);
    Task<User?> GetUserWithRoles(string username, Guid? securityToken = null);
    Task<bool> UserExistsWithFollowing(string userDtoUsername, string userDtoEmail);
    Task Save(User user);
}