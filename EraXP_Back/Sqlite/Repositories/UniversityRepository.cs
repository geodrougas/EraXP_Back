using EraXP_Back.Models.Database;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.Repositories;

namespace EraXP_Back.Sqlite.Repositories;

public class UniversityRepository(IDbExec dbExec) : IUniversityRepository
{
    private readonly IDbExec _dbExec = dbExec;
    
    public Task<List<University>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<UniversityPhoto>> GetPhoto(Guid? id = null, Guid? uniId = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<University>> Get(Guid? id = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<Address>> GetAddress(Guid? uniId = null)
    {
        throw new NotImplementedException();
    }
}