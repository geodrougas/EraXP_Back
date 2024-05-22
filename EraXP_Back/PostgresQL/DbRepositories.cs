using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.PostgresQL.Repositories;
using EraXP_Back.Repositories;

namespace EraXP_Back.PostgresQL;

public abstract class DbRepositories : DbCrud, IDbRepositories
{
    private ICourseRepository? _courseRepository;
    private IDepartmentRepository? _departmentRepository;
    private IUniversityRepository? _universityRepository;
    private IUserRepository? _userRepository;
    
    public ICourseRepository CourseRepository => _courseRepository ??= new CourseRepository(this);
    public IDepartmentRepository DepartmentRepository => _departmentRepository ??= new DepartmentRepository(this);
    public IUniversityRepository UniversityRepository => _universityRepository ??= new UniversityRepository(this);
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(this);
}