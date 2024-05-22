using EraXP_Back.Models;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Domain;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[ApiController]
[Route("/api/v1/[controller]")]
public class DepartmentController(
    IDbConnectionFactory connectionFactory,
    ClaimUtils claimUtils,
    AuthorityUtils authorityUtils) : ControllerBase
{
    private Authority? _authority;
    private Authority? Authority => _authority ??= claimUtils.GetAuthority(User);
    
    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAllDepartments()
    {
        List<Department> departments;
        
        using (IDbConnection connection = await connectionFactory.ConnectAsync()) {
             departments = await connection.DepartmentRepository.GetAllDepartments();
        }
        
        return Ok(departments.Select(DepartmentDto.From));
    }
    
    [HttpGet]
    [Route("available")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<List<DepartmentDto>>> GetMappedDepartments()
    {
        List<Department> departments;
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
                return StatusCode(result.Code!.Value, result.Error!);

            User user = result.Entity!;
            
            departments = await connection.DepartmentRepository.GetMappedDepartments(user.DepartmentId);
        }

        return Ok(departments.Select(DepartmentDto.From));
    }

    [HttpGet]
    [Route("{departmentId}")]
    public async Task<ActionResult<DepartmentDto>> GetDepartmentById([FromRoute] Guid departmentId)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            return await GetDepartmentById(departmentId);
        }
    }

    [HttpGet]
    [Authorize("student,professor")]
    [Route("my")]
    public async Task<ActionResult<DepartmentDto>> GetMyDepartment()
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
                return StatusCode(result.Code!.Value, result.Error!);

            User user = result.Entity!;

            return await GetDepartmentById(connection, user.DepartmentId);
        }
    }

    private async Task<ActionResult<DepartmentDto>> GetDepartmentById(IDbConnection connection, Guid departmentId)
    {
        var department = (await connection.DepartmentRepository.Get(id: departmentId)).FirstOrDefault();

        if (department == null)
            return NotFound();

        return Ok(DepartmentDto.From(department));
    }
}