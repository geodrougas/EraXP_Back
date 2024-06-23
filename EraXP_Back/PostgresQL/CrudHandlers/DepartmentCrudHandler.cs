using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;

namespace EraXP_Back.PostgresQL.CrudHandlers;

public class DepartmentCrudHandler(
    IDbExec exec
) : ICrudHandler
{
    public Task<int> Insert(IDbExec dbExec, object obj)
    {
        string sql =
            """
            insert into departments (
                id, university_id, name, description
            ) values (
                @Id, @UniversityId, @Name, @Description
            )
            """;

        return exec.ExecuteAsync(sql, obj);
    }

    public Task<int> Update(IDbExec dbExec, object obj)
    {
        string sql =
            """
            update departments set
                university_id = @UniversityId,
                name = @Name,
                description = @Description
            WHERE
                id = @Id
            """;
        return exec.ExecuteAsync(sql, obj);
    }

    public Task<int> Delete(IDbExec dbExec, object obj)
    {
        string sql =
            """
            delete from departments where id = @Id
            """;
        return exec.ExecuteAsync(sql, obj);
    }
}