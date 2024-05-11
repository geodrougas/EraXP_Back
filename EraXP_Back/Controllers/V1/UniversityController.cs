using EraXP_Back.Models;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Domain;
using EraXP_Back.Models.Dto;
using EraXP_Back.Repositories;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[Route("/api/v1/[Controller]")]
[ApiController]
[Authorize]
public class UniversityController(
    UserClaimUtils claimUtils,
    IUserRepository userRepository,
    IUniversityRepository universityRepository
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> CreateUniversity([FromBody] UniversityDto dto)
    {
        UserClaims userClaims = claimUtils.GetClaims(User.Claims);
        User? user = await userRepository.GetUserWithRoles(userClaims.Username, userClaims.SecurityToken);

        if (user == null)
            return Unauthorized("This token is invalidated, please log in again and retry!");

        Role? professorRole = user.UserRoles.FirstOrDefault(m => m.Name == "Professor");
        if (professorRole == null)
            return Forbid("You do not have the necessary roles to launch this request!");

        University university = new University(dto.Name, dto.ThumbnailUrl, dto.Information);

        await universityRepository.Save(university);
        
        return Ok();
    }

    [HttpPost]
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
        
        UserClaims userClaims = claimUtils.GetClaims(User.Claims);
        User? user = await userRepository.GetUserWithRoles(userClaims.Username, userClaims.SecurityToken);

        if (user == null)
            return Unauthorized("This token is invalidated, please log in again and retry!");

        Role? studentRole = user.UserRoles.FirstOrDefault(m => m.Name == "Student");
        if (studentRole == null)
            return Forbid("You do not have the necessary roles to launch this request!");

        return await GetMappedUniversities(user);
    }

    [HttpGet]
    public async Task<ActionResult<List<Department>>> GetDepartments([FromQuery] bool canGo)
    {
        if (!canGo)
            return await GetAllDepartments();
        
        UserClaims userClaims = claimUtils.GetClaims(User.Claims);
        User? user = await userRepository.GetUserWithRoles(userClaims.Username, userClaims.SecurityToken);

        if (user == null)
            return Unauthorized("This token is invalidated, please log in again and retry!");

        Role? studentRole = user.UserRoles.FirstOrDefault(m => m.Name == "Student");
        if (studentRole == null)
            return Forbid("You do not have the necessary roles to launch this request!");

        return await GetMappedDepartments(user);
    }

    [HttpGet]
    public async Task<ActionResult<UniversityDto>> GetAllForUniversity([FromQuery] Guid university)
    {
        throw new NotImplementedException();
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