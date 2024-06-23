using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.Repositories;
using EraXP_Back.Sqlite.Repositories;
using CourseRepository = EraXP_Back.PostgresQL.Repositories.CourseRepository;
using DepartmentRepository = EraXP_Back.PostgresQL.Repositories.DepartmentRepository;
using UniversityRepository = EraXP_Back.PostgresQL.Repositories.UniversityRepository;
using UserRepository = EraXP_Back.PostgresQL.Repositories.UserRepository;

namespace EraXP_Back.Sqlite;

public abstract class DbRepositories : DbCrud, IDbRepositories
{
    private ICourseRepository? _courseRepository;
    private IDepartmentRepository? _departmentRepository;
    private IUniversityRepository? _universityRepository;
    private IUserRepository? _userRepository;
    private IPhotoRepository? _photoRepository;
    
    public ICourseRepository CourseRepository => _courseRepository ??= new CourseRepository(this);
    public IDepartmentRepository DepartmentRepository => _departmentRepository ??= new DepartmentRepository(this);
    public IUniversityRepository UniversityRepository => _universityRepository ??= new UniversityRepository(this);
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(this);
    public IPhotoRepository PhotoRepository => _photoRepository ??= new PhotoRepository(this);
    public IContactsRepository ContactsRepository { get; }
    public ILanguageRepository LanguageRepository { get; }
}