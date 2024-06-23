namespace EraXP_Back.Models.Database;

public record Course(
    Guid Id, 
    Guid DepartmentId, 
    string Semester,
    string Name, 
    string Description, 
    decimal Ects
);