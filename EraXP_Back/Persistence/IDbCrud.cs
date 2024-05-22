namespace EraXP_Back.Persistence;

public interface IDbCrud
{
    Task<int> Insert(object obj);
    Task<int> Update(object obj);
    Task<int> Delete(object obj);
}