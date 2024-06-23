using EraXP_Back.Models.Database;

namespace EraXP_Back.Persistence.Repositories;

public interface IUniversityRepository
{
    Task<List<UniversityPhoto>> GetPhoto(Guid? id = null, Guid? uniId = null);
    Task<List<University>> Get(Guid? id = null);
    Task<List<Address>> GetAddress(Guid? uniId = null);
}