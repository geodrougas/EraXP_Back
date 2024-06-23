namespace EraXP_Back.Models.Database;

public record Address(
    Guid Id,
    Guid UniversityId,
    string Name,
    string GoogleLocationId,
    decimal Latitude,
    decimal Longitude
);