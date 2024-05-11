namespace EraXP_Back.Models.Domain;

public class UserClaims
{
    public UserClaims(string username, Guid securityToken)
    {
        Username = username;
        SecurityToken = securityToken;
    }

    public string Username { get; private set; }
    public Guid SecurityToken { get; private set; }
}