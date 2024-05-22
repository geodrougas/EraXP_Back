namespace EraXP_Back.Persistence;

public interface IDbTransaction : IDbRepositories, IDbExec, IDbCrud, IDisposable
{
    Task CommitAsync(CancellationToken token = default);
    Task RollbackAsync(string? savepointName, CancellationToken token = default);
    Task SaveAsync(string savepointName, CancellationToken token = default);
}