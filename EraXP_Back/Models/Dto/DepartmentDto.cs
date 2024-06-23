namespace EraXP_Back.Models.Dto;

public record DepartmentDto (
    Guid? Id,
    Guid? UniversityId,
    string Name,
    string Description,
    List<ContactDto>? Contacts,
    List<CourseDto>? Courses
)
{
    public static DepartmentDto From(
        Department department,
        List<ContactDto>? contacts = null,
        List<CourseDto>? courses = null
    )
    {
        return new DepartmentDto(
            department.Id, 
            department.UniversityId, 
            department.Name, 
            department.Description,
            contacts,
            courses
        );
    }

    public Department ToDomain()
    {
        return new Department(Id ?? Guid.NewGuid(), UniversityId!.Value, Name, Description);
    }
}