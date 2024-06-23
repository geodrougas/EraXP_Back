using System.Data.Common;
using EraXP_Back.Models.Database;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.Persistence.Utils;

namespace EraXP_Back.PostgresQL.Repositories;

public class PhotoRepository(
    IDbExec dbExec
) : IPhotoRepository
{
    public async Task<Photo?> Get(Guid id)
    {
        string sql =
            """SELECT * FROM photos WHERE id = @Id""";
        
        await using DbDataReader reader = await dbExec.QueryAsync(sql, new { Id = id });
        List<Photo> photos = await GetPhotosFromReaderAsync(reader);

        if (photos.Count == 0)
            return null;

        return photos[0];
    }

    public async Task<List<Photo>> GetAll(Guid? id = null)
    {
        QueryBuilder builder = new QueryBuilder('@');

        if (id != null)
        {
            builder.Add("id", "Id", id);
        }

        string sql =
            $"""SELECT * FROM photos {builder}""";

        await using DbDataReader reader = await dbExec.QueryAsync(sql, builder.DbParams);
        List<Photo> photos = await GetPhotosFromReaderAsync(reader);

        return photos;
    }

    private async Task<List<Photo>> GetPhotosFromReaderAsync(DbDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("id");
        int nameOrdinal = reader.GetOrdinal("name");
        int uriOrdinal = reader.GetOrdinal("uri");

        List<Photo> photos = new List<Photo>();
        while (await reader.ReadAsync())
        {
            Photo photo = new Photo(
                reader.GetGuid(idOrdinal),
                reader.GetString(nameOrdinal),
                reader.GetString(uriOrdinal)
            );
            
            photos.Add(photo);
        }

        return photos;
    }
}