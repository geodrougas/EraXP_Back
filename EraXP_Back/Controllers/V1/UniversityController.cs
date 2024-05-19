using System.Diagnostics;
using EraXP_Back.Models;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Domain;
using EraXP_Back.Models.Dto;
using EraXP_Back.Repositories;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[Route("/api/v1/[Controller]")]
[ApiController]
public class UniversityController(
    UserClaimUtils claimUtils,
    IUserRepository userRepository,
    IUniversityRepository universityRepository
) : ControllerBase
{
    private Authority? _authority;
    private Authority? Authority => _authority ??= claimUtils.GetAuthority(User);
    
    [HttpPost]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult> CreateUniversity([FromBody] UniversityDto dto)
    {
        if (Authority == null)
            return BadRequest("You must be logged in to access this resource!");
        
        User? user = await userRepository.GetUserWithRoles(Guid.Parse(Authority.Id), Guid.Parse(Authority.Key));

        if (user == null)
            return Unauthorized("This token is invalidated, please log in again and retry!");
        
        University university = new University(dto.Name, dto.ThumbnailUrl, dto.Information);

        await universityRepository.Save(university);
        
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
    public async Task<ActionResult<List<University>>> GetUniversities([FromQuery] bool canGo)
    {
        if (!canGo)
            return await GetAllUniversities();

        if (Authority == null)
            return Unauthorized("You need to login to view this page!");

        if (!Authority.Roles.Contains("student"))
            return Forbid("Only students can view this page!");
        
        User? user = await userRepository.GetUserWithRoles(Guid.Parse(Authority.Id), Guid.Parse(Authority.Key));

        if (user == null)
            return Unauthorized("This token is invalidated, please log in again and retry!");

        Role? studentRole = user.UserRoles.FirstOrDefault(m => m.Name == "Student");
        if (studentRole == null)
            return Forbid("You do not have the necessary roles to launch this request!");

        return await GetMappedUniversities(user);
    }
    
    [HttpGet]
    [Route("{university}")]
    public async Task<ActionResult<UniversityDto>> GetAllForUniversity([FromRoute] Guid university)
    {
        throw new NotImplementedException();
    }


    [HttpGet]
    [Route("department")]
    public async Task<ActionResult<List<Department>>> GetDepartments([FromQuery] bool canGo)
    {
        if (!canGo)
            return await GetAllDepartments();
 
        if (Authority == null)
            return Unauthorized("You need to login to view this page!");

        if (!Authority.Roles.Contains("student"))
            return Forbid("Only students can view this page!");
               
        User? user = await userRepository.GetUserWithRoles(Guid.Parse(Authority.Id), Guid.Parse(Authority.Key));

        if (user == null)
            return Unauthorized("This token is invalidated, please log in again and retry!");

        Role? studentRole = user.UserRoles.FirstOrDefault(m => m.Name == "Student");
        if (studentRole == null)
            return Forbid("You do not have the necessary roles to launch this request!");

        return await GetMappedDepartments(user);
    }

    private async Task<ActionResult<List<University>>> GetAllUniversities()
    {
        List<University> allUniversities = await universityRepository.GetAll();

        return Ok(allUniversities);
    }

    private async Task<ActionResult<List<University>>> GetMappedUniversities(User user)
    {
        List<Guid> departmentGuids = await universityRepository.GetMappedDepartmentGuids(user.DepartmentId);

        List<University> universities = await universityRepository.GetUniversityFromDepartmentIds(departmentGuids);

        return Ok(universities);
    }

    private async Task<ActionResult<List<Department>>> GetMappedDepartments(User user)
    {
        List<Guid> departmentGuids = await universityRepository.GetMappedDepartmentGuids(user.DepartmentId);
        List<Department> departments = await universityRepository.GetDepartmentsByIds(departmentGuids);

        return Ok(departments);
    }
    
    private async Task<ActionResult<List<Department>>> GetAllDepartments()
    {
        List<Department> departments = await universityRepository.GetAllDepartments();

        return Ok(departments);
    }
}