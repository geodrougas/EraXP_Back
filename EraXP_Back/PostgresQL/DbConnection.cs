using System.Data.Common;
using EraXP_Back.Persistence;
using Npgsql;

namespace EraXP_Back.PostgresQL;

public class DbConnection(
    NpgsqlConnection connection
) : DbRepositories, IDbConnection, IDbCrud, IDbExec, IDisposable, IAsyncDisposable
{
    protected override NpgsqlConnection Connection => connection;
    protected override NpgsqlTransaction? Transaction => null;

    
    
    public void Dispose()
    {
        connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await connection.DisposeAsync();
    }
}