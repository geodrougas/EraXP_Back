namespace EraXP_Back.Models;

public class Department
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Department(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}