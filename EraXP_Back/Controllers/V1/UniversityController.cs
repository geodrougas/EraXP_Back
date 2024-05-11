using EraXP_Back.Models.Domain;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[Route("/api/v1/[Controller]")]
[ApiController]
[Authorize]
public class UniversityController(UserClaimUtils claimUtils) : ControllerBase
{
    [HttpGet]
    public ActionResult CreateUniversity()
    {
        UserClaims userClaims = claimUtils.GetClaims(User.Claims);
        return Ok(userClaims.Username);
    }
}