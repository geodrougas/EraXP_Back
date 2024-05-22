using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;
using Npgsql;

namespace EraXP_Back.PostgresQL;

public abstract class DbCrud : DbExec, IDbCrud
{
    protected IDictionary<Type, ICrudHandlers> CrudHandlersMap = new Dictionary<Type, ICrudHandlers>()
    {

    };

    public Task<int> Insert(object obj)
    {
        return CrudHandlersMap[obj.GetType()].Insert(this, obj);
    }

    public Task<int> Update(object obj)
    {
        return CrudHandlersMap[obj.GetType()].Update(this, obj);
    }

    public Task<int> Delete(object obj)
    {
        return CrudHandlersMap[obj.GetType()].Delete(this, obj);
    }
}