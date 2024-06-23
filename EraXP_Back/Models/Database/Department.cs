namespace EraXP_Back.Models;

public record Department (
    Guid Id,
    Guid UniversityId,
    string Name,
    string Description
);