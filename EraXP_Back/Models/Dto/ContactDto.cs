using EraXP_Back.Models.Database;

namespace EraXP_Back.Models.Dto;

public record ContactDto(
    Guid? Id,
    Guid? DepartmentId,
    string Name,
    string Lastname,
    string Email,
    string PhoneNumber
)
{
    public Contact To(Guid departmentId)
    {
        return new Contact(
            Id ?? Guid.NewGuid(), departmentId, Name, Lastname, Email, PhoneNumber
        );
    }

    public static ContactDto From(Contact contact)
    {
        return new ContactDto(
            contact.Id,
            contact.DepartmentId,
            contact.Name,
            contact.Lastname,
            contact.Email, contact.PhoneNumber
        );
    }
}