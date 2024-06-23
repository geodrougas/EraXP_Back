using System.Data.Common;
using Dapper;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Model;
using Npgsql;

namespace EraXP_Back.PostgresQL;

public abstract class DbExec : IDbExec
{
    protected abstract NpgsqlConnection Connection { get; }
    protected abstract NpgsqlTransaction? Transaction { get; }

    public Task<int> ExecuteAsync(string sql, object parameters)
    {
        object paramObj = GetParamObj(parameters);
        
        return Connection.ExecuteAsync(sql, paramObj, Transaction);
    }

    public Task<DbDataReader> QueryAsync(string sql, object parameters)
    {
        var paramObj = GetParamObj(parameters);

        return Connection.ExecuteReaderAsync(sql, paramObj, Transaction);
    }

    private static object GetParamObj(object parameters)
    {
        object paramObj = parameters;
        if (parameters is IEnumerable<DbParam> dbParams)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (DbParam dbParam in dbParams)
            {
                dynamicParameters.Add(dbParam.Name, dbParam.Value);
            }

            paramObj = dynamicParameters;
        }

        return paramObj;
    }
}