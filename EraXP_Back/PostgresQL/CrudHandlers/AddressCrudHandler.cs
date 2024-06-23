using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;

namespace EraXP_Back.PostgresQL.CrudHandlers;

public class AddressCrudHandler(
    IDbExec dbExec
) : ICrudHandler
{
    public Task<int> Insert(IDbExec dbExec, object obj)
    {
        string sql =
            """
            insert into addresses (
                id, university_id, name, google_location_id, latitude, longitude
            ) VALUES (
                @Id, @UniversityId, @Name, @GoogleLocationId, @Latitude, @Longitude
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