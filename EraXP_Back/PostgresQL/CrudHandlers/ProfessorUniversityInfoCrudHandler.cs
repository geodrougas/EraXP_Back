using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;

namespace EraXP_Back.PostgresQL.CrudHandlers;

public class ProfessorUniversityInfoCrudHandler(
    IDbExec dbExec
) : ICrudHandler
{
    public Task<int> Insert(IDbExec dbExec, object obj)
    {
        string sql =
            """
            insert into professor_university_info (
                id, user_id, university_id
            ) VALUES (
                @Id,
                @UserId,
                @UniversityId
            )
            """;

        return dbExec.ExecuteAsync(sql, obj);
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