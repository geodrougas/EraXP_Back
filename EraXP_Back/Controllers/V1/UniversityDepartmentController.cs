using EraXP_Back.Models;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[ApiController]
[Route("/api/v1/university/{UniversityId}/department")]
public class UniversityDepartmentController(
    IDbConnectionFactory connectionFactory
) : ControllerBase
{
    [FromRoute]
    public Guid UniversityId { get; set; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> Get()
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            List<Department> departments = await connection.DepartmentRepository.GetUniversityDepartments(uniId: UniversityId);
            return Ok(departments.Select(m => DepartmentDto.From(m)));
        }
    }

    [HttpGet]
    [Route("complete")]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetComplete()
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            List<Department> departments = await connection.DepartmentRepository.GetUniversityDepartments(uniId: UniversityId);
            List<DepartmentDto> dtos = new List<DepartmentDto>(departments.Count);

            foreach (var department in departments)
            {
                dtos.Add(await GetComplete(connection, department));
            }
            
            return Ok(dtos);
        }
    }

    private async Task<DepartmentDto> GetComplete(IDbConnection connection, Department department)
    {
        List<Course> courses = await connection.CourseRepository.GetDepartmentCoursesAsync(department.Id);
        List<Contact> contacts = await connection.ContactsRepository.Get(depId: department.Id);

        List<CourseDto> courseDtos = courses.Select(CourseDto.From).ToList();
        List<ContactDto> contactDtos = contacts.Select(ContactDto.From).ToList();
        
        return DepartmentDto.From(department, contactDtos, courseDtos);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> Insert([FromBody] DepartmentDto departmentDto)
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            departmentDto = departmentDto with { UniversityId = UniversityId };
            
            Department department = departmentDto.ToDomain();
            int changes = await connection.Insert(department);

            return changes;
        }
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> Update([FromBody] DepartmentDto departmentDto)
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            departmentDto = departmentDto with { UniversityId = UniversityId };

            if (departmentDto.Id == null)
                return BadRequest("Cannot update an unidentified object!");
            
            Department department = departmentDto.ToDomain();

            return await connection.Update(department);
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> Remove([FromBody] DepartmentDto departmentDto)
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            departmentDto = departmentDto with { UniversityId = UniversityId };
            
            if (departmentDto.Id == null)
                return BadRequest("Cannot remove an unidentified object!");

            return await connection.Delete(departmentDto.ToDomain());
        }
    }
}