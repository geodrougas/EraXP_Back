using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;

namespace EraXP_Back.PostgresQL.CrudHandlers;

public class UserCrudHandler(IDbExec dbExec) : ICrudHandler
{
    private IDbExec _dbExec = dbExec;

    public Task<int> Insert(IDbExec dbExec, object obj)
    {
        string sql =
            """
            insert into users(
                id,
                username,
                base64_hashed_password,
                normalized_username,
                email,
                normalized_email,
                user_type,
                security_stamp,
                concurrency_stamp
            ) VALUES (
                @Id,
                @Username,
                @Base64HashedPassword,
                @NormalisedUsername,
                @Email,
                @NormalisedEmail,
                @UserType,
                @SecurityStamp,
                @ConcurrencyStamp
            )
            """;

        return _dbExec.ExecuteAsync(sql, obj);
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