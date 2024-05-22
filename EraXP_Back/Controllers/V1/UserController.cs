using System.Runtime.InteropServices.Marshalling;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.PostgresQL;
using EraXP_Back.Repositories;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[ApiController]
[Route("/api/v1/[Controller]")]
public class UserController(
    DbConnectionFactory connectionFactory,
    ClaimUtils claimUtils,
    UserUtils userUtils
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> CreateUser([FromBody] UserDto userDto)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            bool userExists = await connection.UserRepository.CheckIfUserExistsOr(username: userDto.Username, email: userDto.Email);

            if (userExists)
                return BadRequest("A user already exists with that username or email.");

            if (userDto.Password != userDto.PasswordRepeat)
                return BadRequest("Password mismatch!");

            if (userDto.Password.Length < UserUtils.MIN_PASSWORD_LENGTH)
                return ($"Your password's length must be greater than {UserUtils.MIN_PASSWORD_LENGTH}!");

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

            await connection.Insert(user);
        }
        // save user roles
        
        return Ok();
    }
}