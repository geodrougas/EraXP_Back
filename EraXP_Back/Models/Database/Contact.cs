namespace EraXP_Back.Models.Database;

public record Contact(
    Guid Id,
    Guid DepartmentId,
    string Name,
    string Lastname,
    string Email,
    string PhoneNumber
);