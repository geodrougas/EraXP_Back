namespace EraXP_Back.Models;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public Role(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}