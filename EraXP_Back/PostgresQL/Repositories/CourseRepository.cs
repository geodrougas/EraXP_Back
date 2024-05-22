using EraXP_Back.Persistence;
using EraXP_Back.Repositories;

namespace EraXP_Back.PostgresQL.Repositories;

public class CourseRepository(IDbExec dbExec) : ICourseRepository
{
    private readonly IDbExec _dbExec = dbExec;
}