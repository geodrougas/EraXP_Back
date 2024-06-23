using EraXP_Back.Models.Database;
using EraXP_Back.Models.Domain;
using EraXP_Back.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Utils;

public class AuthorityUtils
{
    public async Task<Result<User>> ValidateAuthority(IDbConnection connection, Authority? authority)
    {
        if (authority == null)
            return (401, "You need to login to view this page!");
        
        User? user = await connection.UserRepository.Get(
            id: Guid.Parse(authority.Id), securityToken: Guid.Parse(authority.Key));

        if (user == null)
            return (401, "This token is invalidated, please log in again and retry!");
        
        return user;
    }
}