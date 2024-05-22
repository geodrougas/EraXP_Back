using Npgsql;

namespace EraXP_Back.Persistence.CrudHandlers;

public interface ICrudHandlers
{
    Task<int> Insert(IDbExec dbExec, object obj);
    Task<int> Update(IDbExec dbExec,  object obj);
    Task<int> Delete(IDbExec dbExec,  object obj);
}