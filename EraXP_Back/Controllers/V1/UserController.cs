using EraXP_Back.Models.Database;
using EraXP_Back.Models.Dto;
using EraXP_Back.Repositories;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[ApiController]
[Route("/api/v1/[Controller]")]
public class UserController(
   UserClaimUtils claimUtils,
   UserUtils userUtils,
   IUserRepository userRepository
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> CreateUser([FromBody] UserDto userDto)
    {
        bool userExists = await userRepository.UserExistsWithFollowing(userDto.Username, userDto.Email);

        if (userExists)
            return BadRequest("A user already exists with that username or email.");
        
        if (userDto.Password != userDto.PasswordRepeat)
            return "Password mismatch!";

        if (userDto.Password.Length < UserUtils.MIN_PASSWORD_LENGTH)
            return $"Your password's length must be greater than {UserUtils.MIN_PASSWORD_LENGTH}!";

        Guid securityToken = Guid.NewGuid();
        Guid concurrencyToken = Guid.NewGuid();
        
        string password = userUtils.CreatePassword(securityToken, userDto.Password, userDto.PasswordRepeat);
        
        User user = new User(
            Guid.NewGuid(),
            userDto.Username,
            password,
            userDto.Username.ToUpperInvariant(),
            userDto.Email,
            userDto.Email.ToUpperInvariant(),
            userDto.UniversityId, 
            userDto.DepartmentId,
            securityToken,
            concurrencyToken,
            false,
            null
        );

        await userRepository.Save(user);

        // save user roles
        
        return Ok();
    }
}