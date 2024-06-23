using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;

namespace EraXP_Back.PostgresQL.CrudHandlers;

public class ContactCrudHandler : ICrudHandler
{
    public Task<int> Insert(IDbExec dbExec, object obj)
    {
        string sql =
            """
            insert into contacts
                (id, department_id, name, lastname, email, phone_number)
            values
                (@Id, @DepartmentId, @Name, @Lastname, @Email, @PhoneNumber)
            """;
        return dbExec.ExecuteAsync(sql, obj);
    }

    public Task<int> Update(IDbExec dbExec, object obj)
    {
        string sql =
            """
            update contacts set
                department_id = @DepartmentId,
                name = @Name,
                lastname = @Lastname,
                email = @Email,
                phone_number = @PhoneNumber
            where
                id = @Id
            """;
        return dbExec.ExecuteAsync(sql, obj);
    }

    public Task<int> Delete(IDbExec dbExec, object obj)
    {
        string sql =
            """DELETE FROM contacts WHERE id = @Id""";

        return dbExec.ExecuteAsync(sql, obj);
    }
}