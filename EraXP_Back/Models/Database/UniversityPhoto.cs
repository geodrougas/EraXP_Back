namespace EraXP_Back.Models.Database;

public record UniversityPhoto (
    Guid Id,
    Guid UniversityId,
    Guid? PhotoId,
    string? Uri
);