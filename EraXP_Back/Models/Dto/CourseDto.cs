using EraXP_Back.Models.Database;

namespace EraXP_Back.Models.Dto;

public record CourseDto(
    Guid? Id, Guid? DepartmentId, string Semester, string Name, string Description, decimal Ects
)
{
    public static CourseDto From(Course course)
    {
        return new CourseDto(
            course.Id, 
            course.DepartmentId, 
            course.Semester,
            course.Name, 
            course.Description, 
            course.Ects
        );
    }

    public Course ToDto()
    {
        return new Course(
            Id ?? Guid.NewGuid(), 
            DepartmentId!.Value, 
            Semester,
            Name, 
            Description, 
            Ects
        );
    }
}