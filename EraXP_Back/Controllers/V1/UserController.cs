using System.Runtime.InteropServices.JavaScript;
using System.Runtime.InteropServices.Marshalling;
using EraXP_Back.Models;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Domain;
using EraXP_Back.Models.Domain.Enum;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.PostgresQL;
using EraXP_Back.Repositories;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EraXP_Back.Controllers.V1;

[ApiController]
[Authorize]
[Route("/api/v1/[Controller]")]
public class UserController(
    IDbConnectionFactory connectionFactory,
    AuthorityUtils authorityUtils,
    ClaimUtils claimUtils,
    UserUtils userUtils
) : ControllerBase
{
    private Authority? _authority;
    private Authority? Authority => _authority ??= claimUtils.GetAuthority(User);
    
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetUser()
    {
        if (Authority == null)
            return Unauthorized("Please log in!");
        
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> userResult = await authorityUtils.ValidateAuthority(connection, Authority);
            if (userResult.Error != null)
                return StatusCode(userResult.Code!.Value, userResult.Error);

            User user = userResult.Entity!;

            Result<IUserUniversityInfo> infosResult =
                await UserUtils.GetUserUniversityInfo(connection, user);

            Guid? universityId = null;
            Guid? departmentId = null;
            
            if (infosResult.IsSuccess)
            {
                IUserUniversityInfo userUniversityInfo = infosResult.Entity!;
                universityId = userUniversityInfo.UniversityId;
                departmentId = userUniversityInfo.DepartmentId;
            }

            return Ok(new UserDto(user.Username, user.Email, user.UserType, universityId, departmentId));
        }
    }

    [HttpPost]
    [Route("logout")]
    public async Task<ActionResult> SignOut()
    {

        if (Authority == null)
            return Ok();

        if (Authority.AuthorizationScheme == CookieAuthenticationDefaults.AuthenticationScheme)
        {
            await HttpContext.SignOutAsync();
        }

        return Ok();
    }
}