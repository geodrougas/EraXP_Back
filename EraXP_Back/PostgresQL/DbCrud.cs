using EraXP_Back.Models;
using EraXP_Back.Models.Database;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;
using EraXP_Back.PostgresQL.CrudHandlers;
using Npgsql;

namespace EraXP_Back.PostgresQL;

public abstract class DbCrud : DbExec, IDbCrud
{
    private readonly IDictionary<Type, ICrudHandler> _crudHandlersMap;

    protected DbCrud()
    {
        _crudHandlersMap = new Dictionary<Type, ICrudHandler>()
        {
            { typeof(User), new UserCrudHandler(this) },
            { typeof(StudentUniversityInfo), new StudentUniversityInfoCrudHandler(this) },
            { typeof(ProfessorUniversityInfo), new ProfessorUniversityInfoCrudHandler(this) },
            { typeof(University), new UniversityCrudHandler(this) },
            { typeof(Address), new AddressCrudHandler(this) },
            { typeof(Photo), new PhotoCrudHandler(this) },
            { typeof(UniversityPhoto), new UniversityPhotoCrudHandler(this) },
            { typeof(Department), new DepartmentCrudHandler(this) },
            { typeof(Course), new CourseCrudHandler() },
            { typeof(Contact), new ContactCrudHandler() }
        };
    }
    
    public Task<int> Insert(object obj)
    {
        return _crudHandlersMap[obj.GetType()].Insert(this, obj);
    }

    public Task<int> Update(object obj)
    {
        return _crudHandlersMap[obj.GetType()].Update(this, obj);
    }

    public Task<int> Delete(object obj)
    {
        return _crudHandlersMap[obj.GetType()].Delete(this, obj);
    }
}