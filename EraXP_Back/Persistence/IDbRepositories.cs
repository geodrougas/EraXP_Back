using EraXP_Back.Persistence.Repositories;
using EraXP_Back.Repositories;

namespace EraXP_Back.Persistence;

public interface IDbRepositories
{
    ICourseRepository CourseRepository { get; }
    IDepartmentRepository DepartmentRepository { get; }
    IUniversityRepository UniversityRepository { get; }
    IUserRepository UserRepository { get; }
    IPhotoRepository PhotoRepository { get; }
    IContactsRepository ContactsRepository { get; }
    ILanguageRepository LanguageRepository { get; }
}