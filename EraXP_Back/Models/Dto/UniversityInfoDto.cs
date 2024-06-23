namespace EraXP_Back.Models.Dto;

public record UniversityInfoDto(
    Guid UniversityId,
    Guid? DepartmentId
);