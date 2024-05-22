namespace EraXP_Back.Models.Dto;

public record DepartmentDto (
    Guid Id,
    string Name,
    string Description
)
{
    public static DepartmentDto From(Department department)
    {
        return new DepartmentDto(department.Id, department.Name, department.Description);
    }
}