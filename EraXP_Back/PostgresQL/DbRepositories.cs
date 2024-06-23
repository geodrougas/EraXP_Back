using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.PostgresQL.Repositories;
using EraXP_Back.Repositories;
using CourseRepository = EraXP_Back.PostgresQL.Repositories.CourseRepository;
using DepartmentRepository = EraXP_Back.PostgresQL.Repositories.DepartmentRepository;
using UniversityRepository = EraXP_Back.PostgresQL.Repositories.UniversityRepository;
using UserRepository = EraXP_Back.PostgresQL.Repositories.UserRepository;

namespace EraXP_Back.PostgresQL;

public abstract class DbRepositories : DbCrud, IDbRepositories
{
    private ICourseRepository? _courseRepository;
    private IDepartmentRepository? _departmentRepository;
    private IUniversityRepository? _universityRepository;
    private IUserRepository? _userRepository;
    private IPhotoRepository? _photoRepository;
    private IContactsRepository? _contactsRepository;
    private ILanguageRepository? _languageRepository;
    
    public ICourseRepository CourseRepository => _courseRepository ??= new CourseRepository(this);
    public IDepartmentRepository DepartmentRepository => _departmentRepository ??= new DepartmentRepository(this);
    public IUniversityRepository UniversityRepository => _universityRepository ??= new UniversityRepository(this);
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(this);
    public IPhotoRepository PhotoRepository => _photoRepository ??= new PhotoRepository(this);
    public IContactsRepository ContactsRepository => _contactsRepository ??= new ContactsRepository(this);
    public ILanguageRepository LanguageRepository => _languageRepository ??= new LanguageRepository(this);
}