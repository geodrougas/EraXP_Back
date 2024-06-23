using EraXP_Back.Models.Database;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using EraXP_Back.PostgresQL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseRepository = EraXP_Back.Sqlite.Repositories.CourseRepository;

namespace EraXP_Back.Controllers.V1;

[ApiController]
[Route("/api/v1/university/{UniversityId}/department/{DepartmentId}/course")]
public class UniversityDepartmentCoursesController(
    IDbConnectionFactory connectionFactory
) : ControllerBase
{
    
    [FromRoute]
    public Guid UniversityId { get; set; }

    [FromRoute]
    public Guid DepartmentId { get; set; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> Get()
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            List<Course> courses = await connection.CourseRepository.GetDepartmentCoursesAsync(depId: DepartmentId);
            return Ok(courses.Select(CourseDto.From));
        }
    }
    
    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> Insert([FromBody] CourseDto courseDto)
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            courseDto = courseDto with { DepartmentId = DepartmentId }; 
            
            Course course = courseDto.ToDto();
            return await connection.Insert(course);
        }
    }

    [HttpPut, Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> Update([FromBody] CourseDto courseDto)
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            courseDto = courseDto with { DepartmentId = DepartmentId };

            if (courseDto.Id == null)
                return BadRequest("Cannot update an unidentified object!");
            
            Course course = courseDto.ToDto();
            return await connection.Update(course);
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> Remove([FromBody] CourseDto courseDto)
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            courseDto = courseDto with { DepartmentId = DepartmentId }; 
            if (courseDto.Id == null)
            {
                return BadRequest("Cannot delete an unidentified object!");
            }
            
            Course course = courseDto.ToDto();
            int changes = await connection.Delete(course);

            return Ok(changes);
        }
    }
}