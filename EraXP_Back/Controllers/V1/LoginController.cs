using EraXP_Back.Models.Database;
using EraXP_Back.Models.Domain;
using EraXP_Back.Models.Dto;
using EraXP_Back.Repositories;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[ApiController]
[Route("/api/v1/[Controller]")]
public class LoginController(
    UserClaimUtils claimUtils,
    UserUtils userUtils,
    IUserRepository userRepository
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> Login([FromBody] CredentialsDto credentialsDto)
    {
        User? user = await userRepository.GetUser(credentialsDto.Username);

        if (user == null)
            return BadRequest("Invalid credentials!");

        bool result = userUtils.ValidatePassword(user, credentialsDto.Password);
        
        if (!result)
            return BadRequest("Invalid credentials!");
        
        return Ok(
            claimUtils.GenerateJsonSignature(
                    new UserClaims("geodrougas", Guid.NewGuid()))
        );
    }
}