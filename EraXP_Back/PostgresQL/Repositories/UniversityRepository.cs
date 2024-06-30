using System.Data.Common;
using EraXP_Back.Models;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.Persistence.Utils;
using EraXP_Back.Repositories;
using EraXP_Back.Utils;

namespace EraXP_Back.PostgresQL.Repositories;

public class UniversityRepository(IDbExec dbExec) : IUniversityRepository
{
    private readonly IDbExec _dbExec = dbExec;

    public async Task<List<UniversityPhoto>> GetPhoto(Guid? id = null, Guid? uniId = null)
    {
        QueryBuilder builder = new QueryBuilder('@');

        if (id != null)
        {
            builder.Add("id", "Id", id);
        }

        if (uniId != null)
        {
            builder.Add("university_id", "UniversityId", uniId);
        }
        
        string sql =
            $"""SELECT * FROM university_photos {builder}""";

        await using DbDataReader reader = await dbExec.QueryAsync(sql, builder.DbParams);
        List<UniversityPhoto> universityPhotos = await GetUniversityPhotosFromReaderAsync(reader);

        return universityPhotos;
    }

    public async Task<List<University>> Get(Guid? id = null)
    {
        QueryBuilder builder = new QueryBuilder('@');

        if (id != null)
        {
            builder.Add("id", "Id", id);
        }
        
        string sql =
            $"""SELECT * FROM universities {builder}""";

        await using DbDataReader reader = await dbExec.QueryAsync(sql, builder.DbParams);
        List<University> universities = await GetUniversitiesFromReaderAsync(reader);

        return universities;
    }

    public async Task<List<Address>> GetAddress(Guid? uniId = null)
    {
        QueryBuilder builder = new QueryBuilder('@');

        if (uniId != null)
        {
            builder.Add("university_id", "UniversityId", uniId);
        }

        string sql =
            $"""SELECT * FROM addresses {builder}""";

        await using DbDataReader reader = await dbExec.QueryAsync(sql, builder.DbParams);
        List<Address> addresses = await GetAddressesFromReader(reader);

        return addresses;
    }

    public Task<int> DeleteUniversityImage(Guid photoId)
    {
        string sql = """
        DELETE FROM university_photos where photo_id = @PhotoId
        """;
        return dbExec.ExecuteAsync(sql, new { PhotoId = photoId });
    }

    private async Task<List<Address>> GetAddressesFromReader(DbDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("id");
        int universityIdOrdinal = reader.GetOrdinal("university_id");
        int nameOrdinal = reader.GetOrdinal("name");
        int googleLocationIdOrdinal = reader.GetOrdinal("google_location_id");
        int latitudeOrdinal = reader.GetOrdinal("latitude");
        int longitudeOrdinal = reader.GetOrdinal("longitude");

        List<Address> addresses = new List<Address>();
        while (await reader.ReadAsync())
        {
            Address address = new Address(
                reader.GetGuid(idOrdinal),
                reader.GetGuid(universityIdOrdinal),
                reader.GetString(nameOrdinal),
                reader.GetString(googleLocationIdOrdinal),
                reader.GetDecimal(latitudeOrdinal),
                reader.GetDecimal(longitudeOrdinal)
            );
            
            addresses.Add(address);
        }

        return addresses;
    }

    public async Task<List<UniversityPhoto>> GetUniversityPhotosFromReaderAsync(DbDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("id");
        int universityIdOrdinal = reader.GetOrdinal("university_id");
        int photoIdOrdinal = reader.GetOrdinal("photo_id");
        int uriOrdinal = reader.GetOrdinal("uri");

        List<UniversityPhoto> universityPhotos = new List<UniversityPhoto>();
        while (await reader.ReadAsync())
        {
            UniversityPhoto photo = new UniversityPhoto(
                reader.GetGuid(idOrdinal),
                reader.GetGuid(universityIdOrdinal),
                await reader.GetOrDefault<Guid?>(photoIdOrdinal),
                await reader.GetOrDefault<string>(uriOrdinal)
            );
            
            universityPhotos.Add(photo);
        }

        return universityPhotos;
    }
    
    public async Task<List<ProfessorUniversityInfo>> GetProfessorUniversityInfosFromReaderAsync(DbDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("id");
        int userIdOrdinal = reader.GetOrdinal("user_id");
        int universityIdOrdinal = reader.GetOrdinal("university_id");

        List<ProfessorUniversityInfo> professorUniversityInfos = new List<ProfessorUniversityInfo>();
        while (await reader.ReadAsync())
        {
            ProfessorUniversityInfo university = new ProfessorUniversityInfo(
                reader.GetGuid(idOrdinal),
                reader.GetGuid(userIdOrdinal),
                reader.GetGuid(universityIdOrdinal)
            );
            
            professorUniversityInfos.Add(university);
        }

        return professorUniversityInfos;
    }
    
    public async Task<List<StudentUniversityInfo>> GetStudentUniversityInfoFromReaderAsync(DbDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("id");
        int userIdOrdinal = reader.GetOrdinal("user_id");
        int universityIdOrdinal = reader.GetOrdinal("university_id");
        int departmentIdOrdinal = reader.GetOrdinal("department_id");

        List<StudentUniversityInfo> suis = new List<StudentUniversityInfo>();
        while (await reader.ReadAsync())
        {
            StudentUniversityInfo sui = new StudentUniversityInfo(
                reader.GetGuid(idOrdinal),
                reader.GetGuid(userIdOrdinal),
                reader.GetGuid(universityIdOrdinal),
                reader.GetGuid(departmentIdOrdinal)
            );
            
            suis.Add(sui);
        }

        return suis;
    }
    
    public async Task<List<University>> GetUniversitiesFromReaderAsync(DbDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("id");
        int nameOrdinal = reader.GetOrdinal("name");
        int descriptionOrdinal = reader.GetOrdinal("description");
        int thumbnailIdOrdinal = reader.GetOrdinal("thumbnail_id");
        int universityLanguageOrdinal = reader.GetOrdinal("university_language");

        List<University> universities = new List<University>();
        while (await reader.ReadAsync())
        {
            University university = new University(
                reader.GetGuid(idOrdinal),
                reader.GetString(nameOrdinal),
                reader.GetString(descriptionOrdinal),
                await reader.GetOrDefault<Guid?>(thumbnailIdOrdinal),
                reader.GetString(universityLanguageOrdinal)
            );
            
            universities.Add(university);
        }

        return universities;
    }
}