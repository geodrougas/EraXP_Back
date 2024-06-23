using EraXP_Back.Models.Database;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;

namespace EraXP_Back.Sqlite.Repositories;

public class PhotoRepository(
    IDbCrud dbRepositories
) : IPhotoRepository
{
    public Task<Photo?> Get(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Photo>> GetAll(Guid? id = null)
    {
        throw new NotImplementedException();
    }
}