namespace EraXP_Back.Models;

public class UserUniversityRating
{
    public string? Username { get; set; }
    public Guid UniversityId { get; set; }
    public int Stars { get; set; }
    public string Comment { get; set; }

    public UserUniversityRating(
        string username, Guid universityId, int stars, string comment)
    {
        Username = username;
        UniversityId = universityId;
        Stars = stars;
        Comment = comment;
    }
}