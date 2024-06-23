using EraXP_Back.Models.Database;

namespace EraXP_Back.Repositories;

public interface ICourseRepository
{
    Task<List<Course>> GetDepartmentCoursesAsync(Guid depId);
}