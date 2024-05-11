namespace EraXP_Back.Models.Dto;

public record UniversityDto(
    Guid? Id,
    string Name,
    string? ThumbnailUrl,
    string Information,
    List<UniversityPhotoDto>? PhotoMetadata,
    List<UniversityUserRatingsDto>? UserRatings,
    List<Department>? Departments,
    List<Guid>? UserDepartments
);
   
