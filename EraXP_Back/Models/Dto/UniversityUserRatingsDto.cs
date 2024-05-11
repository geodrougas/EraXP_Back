namespace EraXP_Back.Models.Dto;

public record UniversityUserRatingsDto
(
    string? Username,
    Guid UniversityId,
    int Stars,
    string Comment
);