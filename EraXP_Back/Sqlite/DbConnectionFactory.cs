using EraXP_Back.Persistence;
using Npgsql;

namespace EraXP_Back.Sqlite;

public class DbConnectionFactory(
    string connectionString) : IDbConnectionFactory
{
    public async Task<IDbConnection> ConnectAsync()
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        return new DbConnection(connection);
    }
}