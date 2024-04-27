namespace EraXP_Back.Models;

public class UniversityPhoto
{
    public Guid UniversityId { get; set; }
    public string ImageUrl { get; set; }

    public UniversityPhoto(Guid universityId, string imageUrl)
    {
        UniversityId = universityId;
        ImageUrl = imageUrl;
    }
}