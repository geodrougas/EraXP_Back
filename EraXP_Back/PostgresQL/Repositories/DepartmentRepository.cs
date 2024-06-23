using System.Data.Common;
using EraXP_Back.Models;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.Repositories;

namespace EraXP_Back.PostgresQL.Repositories;

public class DepartmentRepository(IDbExec dbExec) : IDepartmentRepository
{
    private readonly IDbExec _dbExec = dbExec;

    public Task<List<Department>> Get(Guid? id)
    {
        throw new NotImplementedException();
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