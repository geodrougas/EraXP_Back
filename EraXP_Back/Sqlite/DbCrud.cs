using EraXP_Back.Persistence;
using EraXP_Back.Persistence.CrudHandlers;

namespace EraXP_Back.Sqlite;

public abstract class DbCrud : DbExec, IDbCrud
{
    protected IDictionary<Type, ICrudHandler> CrudHandlersMap = new Dictionary<Type, ICrudHandler>()
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