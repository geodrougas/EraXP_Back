using System.Data.Common;
using Dapper;
using EraXP_Back.Models.Database;
using EraXP_Back.Persistence;
using EraXP_Back.Repositories;

namespace EraXP_Back.Sqlite.Repositories;

public class CourseRepository(IDbExec dbExec) : ICourseRepository
{
    private readonly IDbExec _dbExec = dbExec;
    
    public Task<List<Course>> GetDepartmentCoursesAsync(Guid depId)
    {
        throw new NotImplementedException();
    }
}