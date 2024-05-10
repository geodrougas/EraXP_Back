namespace EraXP_Back.Models;

public class UserRoles
{
    public string Username { get; set; }
    public Guid Role { get; set; }

    public UserRoles(string username, Guid role)
    {
        Username = username;
        Role = role;
    }
}