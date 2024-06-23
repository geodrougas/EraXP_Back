using EraXP_Back.Persistence;
using Npgsql;

namespace EraXP_Back.Sqlite;

public class DbConnection(
    NpgsqlConnection connection
) : DbRepositories, IDbConnection 
{
    protected override NpgsqlConnection Connection => connection;
    protected override NpgsqlTransaction? Transaction => null;

    
    
    public void Dispose()
    {
        connection.Dispose();
    }

    public IDbTransaction BeginTransaction()
    {
        NpgsqlTransaction transaction = Connection.BeginTransaction();
        
        return new DbTransaction(connection, transaction);
    }

    public async ValueTask DisposeAsync()
    {
        await connection.DisposeAsync();
    }
}