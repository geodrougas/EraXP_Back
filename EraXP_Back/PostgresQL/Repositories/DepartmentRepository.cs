using System.Data.Common;
using EraXP_Back.Models;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.Persistence.Utils;
using EraXP_Back.Repositories;

namespace EraXP_Back.PostgresQL.Repositories;

public class DepartmentRepository(IDbExec dbExec) : IDepartmentRepository
{
    private readonly IDbExec _dbExec = dbExec;

    public async Task<List<Department>> Get(Guid? id)
    {
        QueryBuilder queryBuilder = new QueryBuilder('@');

        if (id != null)
        {
            queryBuilder.Add("id", "Id", id);
        }
        
        string sql =
            $"""select * from departments {queryBuilder}""";

        await using DbDataReader reader = await dbExec.QueryAsync(sql, queryBuilder.DbParams);
        return await GetDepartmentsFromReaderAsync(reader);
    }

    public async Task<List<Department>> GetUniversityDepartments(Guid uniId)
    {
        string sql =
            """select * from departments where university_id=@UniversityId""";

        await using DbDataReader reader = await dbExec.QueryAsync(sql, new { UniversityId = uniId });
        return await GetDepartmentsFromReaderAsync(reader);
    }

    private async Task<List<Department>> GetDepartmentsFromReaderAsync(DbDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("id");
        int universityIdOrdinal = reader.GetOrdinal("university_id");
        int nameOrdinal = reader.GetOrdinal("name");
        int descriptionOrdinal = reader.GetOrdinal("description");

        List<Department> departments = new List<Department>();
        while (await reader.ReadAsync())
        {
            Department department = new Department(
                reader.GetGuid(idOrdinal),
                reader.GetGuid(universityIdOrdinal),
                reader.GetString(nameOrdinal),
                reader.GetString(descriptionOrdinal)
            );
            
            departments.Add(department);
        }

        return departments;
    }
}