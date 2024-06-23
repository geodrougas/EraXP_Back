using EraXP_Back.Models.Database;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using EraXP_Back.PostgresQL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseRepository = EraXP_Back.Sqlite.Repositories.CourseRepository;

namespace EraXP_Back.Controllers.V1;

[ApiController]
[Route("/api/v1/university/{UniversityId}/department/{DepartmentId}/contact")]
public class UniversityDepartmentContactsController(
    IDbConnectionFactory connectionFactory
) : ControllerBase
{
    
    [FromRoute]
    public Guid UniversityId { get; set; }

    [FromRoute]
    public Guid DepartmentId { get; set; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContactDto>>> Get()
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            List<Contact> contacts = await connection.ContactsRepository.Get(depId: DepartmentId);
            return Ok(contacts.Select(ContactDto.From));
        }
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> Insert([FromBody] ContactDto contactsDto)
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Contact course = contactsDto.To(DepartmentId);
            return await connection.Insert(course);
        }
    }

    [HttpPut, Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> Update([FromBody] ContactDto contactDto)
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            if (contactDto.Id == null)
                return BadRequest("Cannot update an unidentified object!");
            
            Contact contact = contactDto.To(DepartmentId);
            return await connection.Update(contact);
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> Remove([FromBody] ContactDto contactDto)
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            if (contactDto.Id == null)
            {
                return BadRequest("Cannot update an unidentified object!");
            }
            
            Contact contact = contactDto.To(DepartmentId);
            return await connection.Delete(contact);
        }
    }
}