using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;

namespace EraXP_Back.PostgresQL.CrudHandlers;

public class CourseCrudHandler : ICrudHandler
{
    public Task<int> Insert(IDbExec dbExec, object obj)
    {
        string sql =
            """
            insert into courses
                (id, department_id, semester, name, description, ects)
            values 
                (@Id, @DepartmentId, @Semester, @Name, @Description, @Ects)
            """;

        return dbExec.ExecuteAsync(sql, obj);
    }

    public Task<int> Update(IDbExec dbExec, object obj)
    {
        string sql =
            """
            update courses set
                department_id = @DepartmentId,
                semester = @Semester,
                name = @Name,
                description = @Description,
                ects = @Ects
            where
                id = @Id
            """;
        return dbExec.ExecuteAsync(sql, obj);
    }

    public Task<int> Delete(IDbExec dbExec, object obj)
    {
        string sql =
            """
            delete from courses where id = @Id
            """;
        return dbExec.ExecuteAsync(sql, obj);
    }
}