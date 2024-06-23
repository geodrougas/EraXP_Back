using Npgsql;

namespace EraXP_Back.Persistence;

public interface IDbConnection : IDbRepositories, IDbExec, IDbCrud, IDisposable, IAsyncDisposable
{
    public IDbTransaction BeginTransaction();
}