using EraXP_Back.Models;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;

namespace EraXP_Back.Sqlite.Repositories;

public class DepartmentRepository(IDbExec dbExec) : IDepartmentRepository
{
    private readonly IDbExec _dbExec = dbExec;

    public Task<List<Department>> Get(Guid? id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Department>> GetUniversityDepartments(Guid uniId)
    {
        throw new NotImplementedException();
    }
}