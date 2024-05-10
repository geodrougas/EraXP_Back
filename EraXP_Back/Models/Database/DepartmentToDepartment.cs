namespace EraXP_Back.Models;

public class DepartmentToDepartment
{
    public Guid DepartmentFirst { get; set; }
    public Guid DepartmentSecond { get; set; }

    public DepartmentToDepartment(Guid departmentFirst, Guid departmentSecond)
    {
        DepartmentFirst = departmentFirst;
        DepartmentSecond = departmentSecond;
    }
}