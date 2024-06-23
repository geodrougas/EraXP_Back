using EraXP_Back.Models.Database;

namespace EraXP_Back.Persistence.Repositories;

public interface IContactsRepository
{
    public Task<List<Contact>> Get(Guid? id = null, Guid? depId = null);
}