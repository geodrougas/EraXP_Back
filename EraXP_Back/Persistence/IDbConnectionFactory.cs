namespace EraXP_Back.Persistence;

public interface IDbConnectionFactory
{
    Task<IDbConnection> ConnectAsync();
}