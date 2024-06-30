using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;

namespace EraXP_Back.PostgresQL.CrudHandlers;

public class PhotoCrudHandler(
    IDbExec dbExec
) : ICrudHandler
{
    public Task<int> Insert(IDbExec dbExec, object obj)
    {
        string sql =
            """
            insert into photos (
                id, name, uri
            ) VALUES (
                @Id, @Name, @Uri
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
        string sql =
            """
            delete from 
                photos
            where
                photos.id = @Id
            """;

        return dbExec.ExecuteAsync(sql, obj);
    }
}