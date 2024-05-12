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

        User user = new User(
            userDto.Username,
            userDto.Username.ToUpper(),
            userDto.Email,
            userDto.Email.ToUpper(),
            userDto.UniversityId, userDto.DepartmentId
        );

        userUtils.CreatePassword(user, userDto.Password, userDto.PasswordRepeat);

        user.ConcurrencyStamp = Guid.NewGuid();

        await userRepository.Save(user);

        return Ok();
    }
}