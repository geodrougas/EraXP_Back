namespace EraXP_Back.Persistence;

public interface IDbTransaction : IDbRepositories, IDbExec, IDbCrud, IDisposable, IAsyncDisposable
{
    Task CommitAsync(CancellationToken token = default);
    Task RollbackAsync(string? savepointName = null, CancellationToken token = default);
    Task SaveAsync(string savepointName, CancellationToken token = default);
}