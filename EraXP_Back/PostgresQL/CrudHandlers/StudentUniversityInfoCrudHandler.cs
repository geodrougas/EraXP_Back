using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;

namespace EraXP_Back.PostgresQL.CrudHandlers;

public class StudentUniversityInfoCrudHandler(
    IDbExec dbExec
) : ICrudHandler
{
    private IDbExec _dbExec = dbExec;
    public Task<int> Insert(IDbExec dbExec, object obj)
    {
        string sql =
            """
            insert into student_university_info (
                id,
                user_id,
                university_id,
                department_id
            ) values (
                @Id,
                @UserId,
                @UniversityId,
                @DepartmentId
            )
            """;
        return _dbExec.ExecuteAsync(sql, obj);
    }

    public Task<int> Update(IDbExec dbExec, object obj)
    {
        throw new NotImplementedException();
    }

    public Task<int> Delete(IDbExec dbExec, object obj)
    {
        throw new NotImplementedException();
    }
}