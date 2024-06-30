using EraXP_Back.Models.Database;

namespace EraXP_Back.Models.Dto;

public record UniversityPhotoDto(
    Guid? Id,
    Guid? UniversityId,
    Guid? PhotoId,
    string? Uri
)
{
    public static UniversityPhotoDto From(UniversityPhoto photo, string baseUrl)
    {
        if (photo.PhotoId != null)
        {
            UriBuilder builder = new UriBuilder(new Uri(baseUrl));
            builder.Path = $"/api/v1/photo/{photo.PhotoId.Value}";
            return new UniversityPhotoDto(photo.Id, photo.UniversityId, photo.PhotoId.Value, builder.ToString());
        }

        return new UniversityPhotoDto(photo.Id, photo.UniversityId, null, photo.Uri!);
    }
}
