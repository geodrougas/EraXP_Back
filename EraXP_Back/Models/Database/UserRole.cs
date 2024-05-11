namespace EraXP_Back.Models;

public class UserRole
{
    public string Username { get; set; }
    public Guid Role { get; set; }

    public UserRole(string username, Guid role)
    {
        Username = username;
        Role = role;
    }
}