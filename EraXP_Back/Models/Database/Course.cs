namespace EraXP_Back.Models;

public class Course
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Ects { get; set; }

    public Course(Guid id, string name, string description, decimal ects)
    {
        Id = id;
        Name = name;
        Description = description;
        Ects = ects;
    }
}