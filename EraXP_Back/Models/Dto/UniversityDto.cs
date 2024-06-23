using System.Text.Json;
using EraXP_Back.Models.Database;

namespace EraXP_Back.Models.Dto;

public record UniversityDto(
    Guid? Id,
    string Name,
    string Description,
    UniversityLanguage Language,
    Guid? UserDepartment,
    Guid? ThumbnailId,
    AddressDto? AddressDto,
    List<UniversityPhotoDto>? PhotoMetadata,
    List<DepartmentDto>? Departments
)
{
    public University To()
    {
        return new University(
            Id ?? Guid.NewGuid(),
            Name,
            Description,
            ThumbnailId,
            JsonSerializer.Serialize(Language)
        );
    }
    
    public static UniversityDto From(
        University university, 
        Address? address = null,
        Guid? userDepartment = null,
        List<UniversityPhotoDto>? photos = null, 
        List<DepartmentDto>? departments = null
    )
    {
        // Todo: Create dto's from the above entities and add them to the university dto
        return new UniversityDto(
            university.Id,
            university.Name,
            university.Description,
            JsonSerializer.Deserialize<UniversityLanguage>(university.Language)!,
            null,
            university.ThumbnailId,
            AddressDto.FromOrDefault(address),
            photos,
            departments
        );
    }
}
   
