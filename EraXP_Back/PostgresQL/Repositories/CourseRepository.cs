using System.Data.Common;
using Dapper;
using EraXP_Back.Models.Database;
using EraXP_Back.Persistence;
using EraXP_Back.Repositories;

namespace EraXP_Back.PostgresQL.Repositories;

public class CourseRepository(IDbExec dbExec) : ICourseRepository
{
    private readonly IDbExec _dbExec = dbExec;
    public async Task<List<Course>> GetDepartmentCoursesAsync(Guid depId)
    {
        string sql = """ SELECT * FROM courses WHERE department_id = @DepId """;
        
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("DepId", depId);

        DbDataReader reader = await _dbExec.QueryAsync(sql, parameters);

        await using (reader)
        {
            return await GetCourseByReaderAsync(reader);
        }
    }

    private async Task<List<Course>> GetCourseByReaderAsync(DbDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("id");
        int departmentIdOrdinal = reader.GetOrdinal("department_id");
        int semesterOrdinal = reader.GetOrdinal("semester");
        int nameOrdinal = reader.GetOrdinal("name");
        int descriptionOrdinal = reader.GetOrdinal("description");
        int ectsOrdinal = reader.GetOrdinal("ects");
        
        List<Course> courses = new List<Course>();
        while (await reader.ReadAsync())
        {
            Course course = new Course(
                reader.GetGuid(idOrdinal),
                reader.GetGuid(departmentIdOrdinal),
                reader.GetString(semesterOrdinal),
                reader.GetString(nameOrdinal),
                reader.GetString(descriptionOrdinal),
                reader.GetDecimal(ectsOrdinal)
            );
            courses.Add(course);
        }

        return courses;
    }
}