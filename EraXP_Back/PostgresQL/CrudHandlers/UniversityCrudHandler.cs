using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;

namespace EraXP_Back.PostgresQL.CrudHandlers;

public class UniversityCrudHandler(
    IDbExec dbExec
) : ICrudHandler
{
    public Task<int> Insert(IDbExec dbExec, object obj)
    {
        string sql =
            """
            insert into universities (
                id, name, description, university_language, thumbnail_id
            ) VALUES (
                @Id, @Name, @Description, @Language, @ThumbnailId
            )
            """;

        return dbExec.ExecuteAsync(sql, obj);
    }

    public Task<int> Update(IDbExec dbExec, object obj)
    {
        string sql =
            """
            update universities set
                name = @Name,
                description = @Description,
                university_language = @Language,
                thumbnail_id = @ThumbnailId
            WHERE
                id = @Id
            """;
        return dbExec.ExecuteAsync(sql, obj);
    }

    public Task<int> Delete(IDbExec dbExec, object obj)
    {
        string sql =
            """DELETE FROM universities WHERE id = @Id""";
        return dbExec.ExecuteAsync(sql, obj);
    }
}