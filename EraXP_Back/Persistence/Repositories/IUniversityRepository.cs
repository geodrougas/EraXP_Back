using EraXP_Back.Models;

namespace EraXP_Back.Repositories;

public interface IUniversityRepository
{
    Task<List<University>> GetAll();
    Task<List<University>> GetUniversityFromDepartmentIds(List<Guid> departmentGuids);
    Task<List<University>> GetAvailableUniversities(Guid userDepartmentId);
    Task<University?> Get(Guid? id = null);
}