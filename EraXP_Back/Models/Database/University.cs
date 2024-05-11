namespace EraXP_Back.Models;

public class University
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string Information { get; set; }

    public University(Guid id, string name, string thumbnailUrl, string information)
    {
        Id = id;
        Name = name;
        ThumbnailUrl = thumbnailUrl;
        Information = information;
    }

    public University(string name, string? thumbnailUrl, string information)
    {
        Id = Guid.NewGuid();
        Name = name;
        ThumbnailUrl = thumbnailUrl;
        Information = information;
    }
}