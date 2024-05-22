using EraXP_Back.Models;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.Repositories;

namespace EraXP_Back.PostgresQL.Repositories;

public class DepartmentRepository(IDbExec dbExec) : IDepartmentRepository
{
    private readonly IDbExec _dbExec = dbExec;

    public Task<List<DepartmentToDepartment>> GetDepartmentMap(Guid userDepartmentId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Department>> GetDepartmentsByIds(List<Guid> departmentGuids)
    {
        throw new NotImplementedException();
    }

    public Task<List<Department>> GetAllDepartments()
    {
        throw new NotImplementedException();
    }

    public Task<List<Department>> GetMappedDepartments(Guid departmentId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Department>> Get(Guid? id)
    {
        throw new NotImplementedException();
    }
}