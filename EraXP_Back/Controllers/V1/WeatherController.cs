using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[Route("/api/v1/[Controller]")]
public class WeatherController : ControllerBase
{
    private string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
    [HttpGet]
    public ActionResult<IEnumerable<WeatherForecast>> Get()
    {
        var forecast =  Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)]
                ))
            .ToArray();
        return forecast;
    }
}