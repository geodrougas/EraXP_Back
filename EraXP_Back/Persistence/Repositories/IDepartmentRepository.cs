using EraXP_Back.Models;

namespace EraXP_Back.Persistence.Repositories;

public interface IDepartmentRepository
{
    Task<List<DepartmentToDepartment>> GetDepartmentMap(Guid userDepartmentId);
    Task<List<Department>> GetDepartmentsByIds(List<Guid> departmentGuids);
    Task<List<Department>> GetAllDepartments();
    Task<List<Department>> GetMappedDepartments(Guid departmentId);
    Task<List<Department>> Get(Guid? id);
}