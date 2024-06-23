using System.Data;
using System.Data.Common;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Domain.Enum;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.Persistence.Utils;
using EraXP_Back.Repositories;
using EraXP_Back.Utils;

namespace EraXP_Back.PostgresQL.Repositories;

public class UserRepository(IDbExec dbExec) : IUserRepository
{
    private readonly IDbExec _dbExec = dbExec;

    public async Task<User?> Get(Guid? id = null, string? username = null, Guid? securityToken = null)
    {
        QueryBuilder builder = new QueryBuilder('@');

        if (id != null)
        {
            builder.Add("id", "Id", id);
        }
        
        builder.And();
        
        if (username != null)
        {
            builder.Add("normalized_username", "Username", username.ToUpper());
        }
        
        builder.And();

        if (securityToken != null)
        {
            builder.Add("security_stamp", "SecurityToken", securityToken);
        }
        
        string sql =
            $"""SELECT * FROM users {builder}""";

        await using DbDataReader reader = await dbExec.QueryAsync(sql, builder.DbParams);
        List<User> users = await GetUsersFromReaderAsync(reader);
            
        if (users.Count == 0)
            return null;
            
        return users[0];
    }

    public async Task<ProfessorUniversityInfo?> GetProfessorsUniversityInfo(Guid userId)
    {
        string sql =
            """SELECT * FROM professor_university_info WHERE user_id = @UserId""";

        await using DbDataReader reader = await dbExec.QueryAsync(sql, new { UserId = userId });
        List<ProfessorUniversityInfo> professorUniversityInfos =
            await GetProfessorUniversityInfosFromReaderAsync(reader);

        if (professorUniversityInfos.Count == 0)
            return null;

        return professorUniversityInfos[0];
    }

    public async Task<StudentUniversityInfo?> GetStudentUniversityInfo(Guid userId)
    {
        string sql =
            """SELECT * FROM student_university_info WHERE user_id = @UserId""";

        await using DbDataReader reader = await dbExec.QueryAsync(sql, new { UserId = userId });
        List<StudentUniversityInfo> studentUniversityInfos =
            await GetStudentUniversityInfosFromReaderAsync(reader);

        if (studentUniversityInfos.Count == 0)
            return null;

        return studentUniversityInfos[0];
    }

    private async Task<List<ProfessorUniversityInfo>> GetProfessorUniversityInfosFromReaderAsync(DbDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("id");
        int userIdOrdinal = reader.GetOrdinal("user_id");
        int universityIdOrdinal = reader.GetOrdinal("university_id");

        List<ProfessorUniversityInfo> infos = new List<ProfessorUniversityInfo>();
        while(await reader.ReadAsync())
        {
            ProfessorUniversityInfo info = new ProfessorUniversityInfo(
                reader.GetGuid(idOrdinal),
                reader.GetGuid(userIdOrdinal),
                reader.GetGuid(universityIdOrdinal)
            );
            infos.Add(info);
        }

        return infos;
    }

    private async Task<List<StudentUniversityInfo>> GetStudentUniversityInfosFromReaderAsync(DbDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("id");
        int userIdOrdinal = reader.GetOrdinal("user_id");
        int universityIdOrdinal = reader.GetOrdinal("university_id");
        int departmentIdOrdinal = reader.GetOrdinal("department_id");
        int erasmusScoreIdOrdinal = reader.GetOrdinal("erasmus_score");

        List<StudentUniversityInfo> infos = new List<StudentUniversityInfo>();
        while(await reader.ReadAsync())
        {
            StudentUniversityInfo info = new StudentUniversityInfo(
                reader.GetGuid(idOrdinal),
                reader.GetGuid(userIdOrdinal),
                reader.GetGuid(universityIdOrdinal),
                reader.GetGuid(departmentIdOrdinal)
            );
            infos.Add(info);
        }

        return infos;
    }

    public async Task<List<User>> GetUsersFromReaderAsync(DbDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("id");
        int usernameOrdinal = reader.GetOrdinal("username");
        int passwordOrdinal = reader.GetOrdinal("base64_hashed_password");
        int normUsernameOrdinal = reader.GetOrdinal("normalized_username");
        int emailOrdinal = reader.GetOrdinal("email");
        int normEmailOrdinal = reader.GetOrdinal("normalized_email");
        int userTypeOrdinal = reader.GetOrdinal("user_type");
        int secStampOrdinal = reader.GetOrdinal("security_stamp");
        int concStampOrdinal = reader.GetOrdinal("concurrency_stamp");

        List<User> users = new List<User>();
        
        while (await reader.ReadAsync())
        {
            User user = new User(
                reader.GetGuid(idOrdinal),
                reader.GetString(usernameOrdinal),
                reader.GetString(passwordOrdinal),
                reader.GetString(normUsernameOrdinal),
                reader.GetString(emailOrdinal),
                reader.GetString(normEmailOrdinal),
                (UserType)reader.GetInt32(userTypeOrdinal),
                reader.GetGuid(secStampOrdinal),
                reader.GetGuid(concStampOrdinal)
            );
            
            users.Add(user);
        }

        return users;
    }
}