using EraXP_Back.Models;

namespace EraXP_Back.Persistence.Repositories;

public interface IDepartmentRepository
{
    Task<List<Department>> Get(Guid? id = null);
    Task<List<Department>> GetUniversityDepartments(Guid uniId);
}