namespace EraXP_Back.Models.Database;

public record University (
    Guid Id,
    string Name,
    string Description,
    Guid? ThumbnailId,
    string Language
);