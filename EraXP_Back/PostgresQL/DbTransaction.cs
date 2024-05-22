using EraXP_Back.Persistence;
using Npgsql;

namespace EraXP_Back.PostgresQL;

public class DbTransaction (
    NpgsqlConnection connection, NpgsqlTransaction transaction)
        :DbRepositories, IDbTransaction, IDbCrud, IDbExec, IAsyncDisposable
{
    protected override NpgsqlConnection Connection => connection;
    protected override NpgsqlTransaction? Transaction => transaction;

    public Task CommitAsync(CancellationToken token = default)
    {
        return transaction.CommitAsync(token);
    }

    public Task RollbackAsync(string? savepointName, CancellationToken token = default)
    {
        if (savepointName != null)
        {
            return transaction.RollbackAsync(savepointName, token);
        }
        return transaction.RollbackAsync(token);
    }

    public Task SaveAsync(string savepointName, CancellationToken token = default)
    {
        return transaction.SaveAsync(savepointName, token);
    }
    
    public void Dispose()
    {
        transaction.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await transaction.DisposeAsync();
    }
}