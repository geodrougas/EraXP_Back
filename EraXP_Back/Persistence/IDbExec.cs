using System.Data.Common;
using EraXP_Back.Persistence.Model;

namespace EraXP_Back.Persistence;

public interface IDbExec
{
    Task<int> ExecuteAsync(string sql, object parameters);
    Task<DbDataReader> QueryAsync(string sql, object parameters);
}