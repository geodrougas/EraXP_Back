using EraXP_Back.Models.Database;
using EraXP_Back.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[ApiController]
[Route("/api/v1/[Controller]")]
public class LanguageController(
    IDbConnectionFactory connectionFactory
)
{
    [HttpGet]
    public async Task<List<Language>> Get([FromQuery] Guid? id)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            return await connection.LanguageRepository.Get(id: id);
        }
    }
}