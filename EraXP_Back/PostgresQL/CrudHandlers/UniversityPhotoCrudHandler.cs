using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;

namespace EraXP_Back.PostgresQL.CrudHandlers;

public class UniversityPhotoCrudHandler(
    IDbExec dbExec
) : ICrudHandler
{
    public Task<int> Insert(IDbExec dbExec, object obj)
    {
        string sql =
            """
            insert into university_photos (
                id, university_id, photo_id, uri
            ) VALUES (
                @Id,
                @UniversityId,
                @PhotoId,
                @Uri
            );
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