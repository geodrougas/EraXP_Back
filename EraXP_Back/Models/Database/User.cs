namespace EraXP_Back.Models;

public class User
{
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    public string NormalisedUsername { get; set; }
    public string Email { get; set; }
    public string NormalisedEmail { get; set; }
    public Guid UniversityId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid SecurityStamp { get; set; }
    public Guid ConcurrencyStamp { get; set; }

    public User(string username, string hashedPassword, string normalisedUsername, string email, string normalisedEmail,
        Guid universityId, Guid departmentId, Guid securityStamp, Guid concurrencyStamp)
    {
        Username = username;
        HashedPassword = hashedPassword;
        NormalisedUsername = normalisedUsername;
        Email = email;
        NormalisedEmail = normalisedEmail;
        UniversityId = universityId;
        DepartmentId = departmentId;
        SecurityStamp = securityStamp;
        ConcurrencyStamp = concurrencyStamp;
    }
}