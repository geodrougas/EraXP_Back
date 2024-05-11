using EraXP_Back.Models.Domain;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[ApiController]
[Route("/api/v1/[Controller]")]
public class LoginController(UserClaimUtils claimUtils) : ControllerBase
{
    [HttpPost]
    public ActionResult<string> Login()
    {
        return claimUtils.GenerateJsonSignature(
            new UserClaims("geodrougas", Guid.NewGuid()));
    }
}