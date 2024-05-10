namespace EraXP_Back.Models;

public class Roles
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public Roles(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}