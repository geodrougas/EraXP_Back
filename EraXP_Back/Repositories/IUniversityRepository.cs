using EraXP_Back.Models;

namespace EraXP_Back.Repositories;

public interface IUniversityRepository
{
    Task Save(University university);
    Task<List<University>> GetAll();
    Task<List<Guid>> GetMappedDepartmentGuids(Guid userDepartmentId);
    Task<List<University>> GetUniversityFromDepartmentIds(List<Guid> departmentGuids);
    Task<List<Department>> GetDepartmentsByIds(List<Guid> departmentGuids);
    Task<List<Department>> GetAllDepartments();
}