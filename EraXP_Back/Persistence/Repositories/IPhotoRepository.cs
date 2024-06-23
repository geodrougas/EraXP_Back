using EraXP_Back.Models.Database;

namespace EraXP_Back.Persistence.Repositories;

public interface IPhotoRepository
{
    public Task<Photo?> Get(Guid id);
    public Task<List<Photo>> GetAll(Guid? id = null);
}