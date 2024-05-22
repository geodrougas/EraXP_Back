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
)
{
    public static UniversityDto? From(University university, 
        List<UniversityPhoto>? photos = null, List<UserUniversityRating>? userRatings = null, List<Department>? departments = null, List<Guid>? userDepartment = null)
    {
        // Todo: Create dto's from the above entities and add them to the university dto
        return new UniversityDto(
            university.Id,
            university.Name,
            university.ThumbnailUrl,
            university.Information,
            null, null, null, null
        );
    }
}
   
