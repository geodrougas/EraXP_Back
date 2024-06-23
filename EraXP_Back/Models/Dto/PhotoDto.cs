using EraXP_Back.Models.Database;

namespace EraXP_Back.Models.Dto;

public record PhotoDto(
    Guid? Id,
    string Name,
    string Uri
)
{
    public Photo To()
    {
        return new Photo(
            Id ?? Guid.NewGuid(),
            Name,
            Uri
        );
    }

    public static PhotoDto From(Photo photo)
    {
        return new PhotoDto(
            photo.Id,
            photo.Name,
            photo.Uri
        );
    }
}