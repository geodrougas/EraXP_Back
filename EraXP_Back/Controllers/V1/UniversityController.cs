using EraXP_Back.Models;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Domain;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[Route("/api/v1/[Controller]")]
[ApiController]
public class UniversityController(
    IDbConnectionFactory connectionFactory,
    AuthorityUtils authorityUtils,
    ClaimUtils claimUtils
) : ControllerBase
{
    private Authority? _authority;
    private Authority? Authority => _authority ??= claimUtils.GetAuthority(User);
    
    [HttpPost]
    [Authorize(Roles = "professor")]
    public async Task<ActionResult> CreateUniversity([FromBody] UniversityDto dto)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Code!.Value, result.Error!);
            }
            
            University university = new University(dto.Name, dto.ThumbnailUrl, dto.Information);

            await connection.Insert(university);
        }
        
        return Ok();
    }

    [HttpPost]
    [Route("photo")]
    public async Task<ActionResult> CreateUniversityPhoto([FromQuery] Guid universityId, [FromBody] List<FormFile> image)
    {
        // Upload a file to a service or store somewhere?
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("available")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<IEnumerable<UniversityDto>>> GetUniversities([FromQuery] bool canGo)
    {
        List<University> universities;
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Code!.Value, result.Error!);
            }

            User user = result.Entity!;
            
            universities = await connection.UniversityRepository.GetAvailableUniversities(user.DepartmentId);
        }

        return Ok(universities.Select(m => UniversityDto.From(m)));
    }
    
    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<IEnumerable<UniversityDto>>> GetAllUniversities()
    {
        List<University> allUniversities;
        
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
             allUniversities = await connection.UniversityRepository.GetAll();
        }

        return Ok(allUniversities.Select(m => UniversityDto.From(m)));
    }
    
    [HttpGet]
    [Route("{universityId}")]
    public async Task<ActionResult<UniversityDto>> GetUniversity([FromRoute] Guid universityId)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            return await GetUniversityConnected(connection, universityId);
        }
    }

    [HttpGet]
    [Route("{universityId}/complete")]
    public async Task<ActionResult<UniversityDto>> GetAllForUniversity([FromRoute] Guid universityId)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            return await GetAllForUniversityConnected(connection, universityId);
        }
    }


    [HttpGet]
    [Route("my")]
    public async Task<ActionResult<UniversityDto>> GetMyUniversity()
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Code!.Value, result.Error!);
            }

            User user = result.Entity!;

            return await GetUniversityConnected(connection, user.UniversityId);
        }
    }
    
    [HttpGet]
    [Route("my/complete")]
    public async Task<ActionResult<UniversityDto>> GetMyUniversityComplete()
    {
        University? university;
        
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Code!.Value, result.Error!);
            }

            User user = result.Entity!;

            return await GetAllForUniversityConnected(connection, user.UniversityId);
        }
    }

    private async Task<ActionResult<UniversityDto>> GetAllForUniversityConnected(IDbConnection connection, Guid universityId)
    {
        University? university;
        
        university = await connection.UniversityRepository.Get(id: universityId);

        if (university == null)
            return NotFound();
        
        // Todo read complete data for university!

        UniversityDto? universityDto = UniversityDto.From(university);
        
        return Ok(universityDto);
    }

    private async Task<ActionResult<UniversityDto>> GetUniversityConnected(IDbConnection connection, Guid universityId)
    {
        University? university;
        
        university = await connection.UniversityRepository.Get(id: universityId);

        if (university == null)
            return NotFound();
        
        UniversityDto? universityDto = UniversityDto.From(university);
        
        return Ok(universityDto);
    }
}