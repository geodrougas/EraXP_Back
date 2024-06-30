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
    [AllowAnonymous]
    [Route("all")]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAllDepartments()
    {
        List<Department> departments;

        await using (IDbConnection connection = await connectionFactory.ConnectAsync()) {
             departments = await connection.DepartmentRepository.Get();
        }
        
        return Ok(departments.Select(m => DepartmentDto.From(m)));
    }

    [HttpGet]
    [Route("{departmentId}")]
    public async Task<ActionResult<DepartmentDto>> GetDepartmentById([FromRoute] Guid departmentId)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            var department = (await connection.DepartmentRepository.Get(id: departmentId)).FirstOrDefault();

            if (department == null)
                return NotFound();

            return Ok(DepartmentDto.From(department));
        }
    }
}