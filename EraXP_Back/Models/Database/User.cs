namespace EraXP_Back.Models.Database;

public class User
{
    public string Username { get; set; }
    public string? Base64HashedPassword { get; set; }
    public string NormalisedUsername { get; set; }
    public string Email { get; set; }
    public string NormalisedEmail { get; set; }
    public Guid UniversityId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid? SecurityStamp { get; set; }
    public Guid? ConcurrencyStamp { get; set; }

    public bool RolesLoaded { get; set; } = false;
    public List<Roles> UserRoles { get; set; }

    public User(string username, string normalisedUsername, string email, string normalisedEmail,
        Guid universityId, Guid departmentId)
    {
        Username = username;
        NormalisedUsername = normalisedUsername;
        Email = email;
        NormalisedEmail = normalisedEmail;
        UniversityId = universityId;
        DepartmentId = departmentId;
        UserRoles = new List<Roles>();
    }
    
    
}