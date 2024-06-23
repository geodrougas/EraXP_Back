using EraXP_Back.Models.Database;
using EraXP_Back.Persistence.Repositories;

namespace EraXP_Back.PostgresQL.Repositories;

public class LanguageRepository : ILanguageRepository
{
    private readonly DbRepositories _dbRepositories;

    private static List<Language> Languages =
    [
        new Language(Guid.Parse("3914f0c2-2837-4eb6-a3fb-94f2e655e864"), "Bengali"),
        new Language(Guid.Parse("60f65f62-3f4d-4f5a-8ab1-fc1e56d4856c"), "Bhojpuri"),
        new Language(Guid.Parse("3c304613-efb8-45da-9b77-20fbe6a047fe"), "Egyptian Arabic"),
        new Language(Guid.Parse("3297e789-466b-40cc-bb59-9b5550d2ac13"), "English"),
        new Language(Guid.Parse("ced3b92e-b276-47b8-913c-9b9107b09561"), "French"),
        new Language(Guid.Parse("68b55625-3511-4bdb-a4cc-911372898c9d"), "German"),
        new Language(Guid.Parse("ffb98158-8d72-497b-a970-be97bb47320d"), "Greek"),
        new Language(Guid.Parse("7567e793-997e-4cd8-8977-27cf6e769890"), "Gujarati"),
        new Language(Guid.Parse("6d142cc9-2ebf-40cb-9f65-f590ba74b6ad"), "Hausa"),
        new Language(Guid.Parse("841a5489-a135-4c96-8b2a-cd56639e1bb7"), "Hindi"),
        new Language(Guid.Parse("cf6d76d8-35df-4afc-9f23-440f7a038326"), "Iranian Persian"),
        new Language(Guid.Parse("1ae4559b-e835-40da-b3f8-9bdfa57d0bc0"), "Italian"),
        new Language(Guid.Parse("910c41e2-c2b8-4f85-96a4-1eab51161d75"), "Japanese"),
        new Language(Guid.Parse("500f4883-65fe-457d-b2df-23aa94f15645"), "Javanese"),
        new Language(Guid.Parse("212d4f66-7c09-4040-ad73-b97280adb7d7"), "Korean"),
        new Language(Guid.Parse("83c6913a-7b35-4b57-bff2-edd83f99ed89"), "Levantine Arabic"),
        new Language(Guid.Parse("a61b402b-a055-4bef-bcf7-4ae1603c7eae"), "Mandarin Chinese"),
        new Language(Guid.Parse("3ce33a92-2382-48ff-a8c5-ee8fa4949333"), "Marathi"),
        new Language(Guid.Parse("92a09f80-843b-4287-982e-4580ca196cb9"), "Portuguese"),
        new Language(Guid.Parse("977303d3-c581-4af1-9da1-c09bf5b8bfbe"), "Russian"),
        new Language(Guid.Parse("ac234cd1-f2bc-43ff-991b-3311ef75f0e1"), "Southern Min"),
        new Language(Guid.Parse("7af03b12-ac27-4558-b7e9-59b7c3f3051e"), "Spanish"),
        new Language(Guid.Parse("ba8ee63b-518f-44ec-ac0c-5f797d4373d5"), "Tamil"),
        new Language(Guid.Parse("7f694ba7-5bbd-4cee-94c1-85d57fe6f4b5"), "Telugu"),
        new Language(Guid.Parse("167879b2-295d-401b-8c81-a47de6982a84"), "Turkish"),
        new Language(Guid.Parse("dfcf23fc-5e62-44ef-9ae5-5a66343ad7d0"), "Urdu"),
        new Language(Guid.Parse("d82d812b-edde-48b8-809f-bdb94f8b9c86"), "Vietnamese"),
        new Language(Guid.Parse("db039422-4311-44f3-a769-1e1100d9e533"), "Western Punjabi"),
        new Language(Guid.Parse("1a31590c-d10d-46b4-9c0a-4b51f1d52f4a"), "Wu Chinese"),
        new Language(Guid.Parse("d5c5e79a-b4a3-4088-a585-ff86b2f5bd76"), "Yue Chinese"),
    ];

    public LanguageRepository(DbRepositories dbRepositories)
    {
        _dbRepositories = dbRepositories;
    }

    public ValueTask<List<Language>> Get(Guid? id = null)
    {
        if (id == null)
        {
            return ValueTask.FromResult(Languages.ToList());
        }

        return ValueTask.FromResult(Languages.Where(it => it.Id == id.Value).ToList());
    }
}