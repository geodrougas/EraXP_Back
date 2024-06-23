using EraXP_Back.Models.Database;

namespace EraXP_Back.Persistence.Repositories;

public interface ILanguageRepository
{
    ValueTask<List<Language>> Get(Guid? id = null);
}