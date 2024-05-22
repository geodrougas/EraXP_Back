using EraXP_Back.Models;
using EraXP_Back.Persistence;
using EraXP_Back.Repositories;

namespace EraXP_Back.PostgresQL.Repositories;

public class UniversityRepository(IDbExec dbExec) : IUniversityRepository
{
    private readonly IDbExec _dbExec = dbExec;
    
    public Task<List<University>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<University>> GetUniversityFromDepartmentIds(List<Guid> departmentGuids)
    {
        throw new NotImplementedException();
    }

    public Task<List<University>> GetAvailableUniversities(Guid userDepartmentId)
    {
        throw new NotImplementedException();
    }

    public Task<University?> Get(Guid? id = null)
    {
        throw new NotImplementedException();
    }
}