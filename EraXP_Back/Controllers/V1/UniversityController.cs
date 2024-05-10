using EraXP_Back.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[Route("/api/v1/[Controller]")]
[ApiController]
public class UniversityController(UserClaimUtils claimUtils) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<WeatherForecast>> CreateUniversity()
    {
        return Ok(claimUtils.ComputeSignature("ok"));
    }
}