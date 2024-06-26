using System.Net.Mime;
using System.Security.Claims;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Domain;
using EraXP_Back.Models.Domain.Enum;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.Repositories;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EraXP_Back.Controllers.V1;

[ApiController]
[Route("/api/v1/[Controller]")]
public class LoginController(
    IDbConnectionFactory connectionFactory,
    ClaimUtils claimUtils,
    UserUtils userUtils
) : ControllerBase
{
    [HttpPost]
    [Route("form")]
    public Task<ActionResult<string>> LoginForm([FromForm] CredentialsDto credentialsDto)
    {
        return LoginBody(credentialsDto);
    }
    
    
    [HttpPost]
    [Route("json")]
    public async Task<ActionResult<string>> LoginBody([FromBody]CredentialsDto credentialsDto)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            User? user = await connection.UserRepository.Get(username: credentialsDto.Username);

            if (user == null)
                return BadRequest("Invalid credentials!");

            bool result = userUtils.ValidatePassword(user, credentialsDto.Password);

            if (!result)
                return BadRequest("Invalid credentials!");

            
            Authority authority = new Authority(
                AuthorizationMethod.GetScheme(credentialsDto.AuthorizationMethod),
                user.Id.ToString(),
                user.SecurityStamp.ToString(),
                EAuthorityType.User,
                [user.UserType.ToString()]
            );

            ClaimsPrincipal principal = claimUtils.GetPrincipal(authority);

            switch (credentialsDto.AuthorizationMethod)
            {
                case EAuthorizationMethod.Cookies:
                {
                    await Response.HttpContext.SignInAsync(
                        authority.AuthorizationScheme,
                        principal,
                        properties: new AuthenticationProperties
                        {
                            ExpiresUtc = DateTimeOffset.Now.Add(claimUtils.AuthLifetime),
                            AllowRefresh = true,
                            IsPersistent = true,
                            IssuedUtc = DateTimeOffset.Now
                        });

                    return Ok();
                }
                default:
                {
                    string token = claimUtils.GenerateJsonSignature(principal);

                    return Ok(token);
                }
            }
        }
    }
}