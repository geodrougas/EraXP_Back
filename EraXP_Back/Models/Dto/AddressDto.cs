using EraXP_Back.Models.Database;

namespace EraXP_Back.Models.Dto;

public record AddressDto(
    Guid? Id,
    string Name,
    string GoogleLocationId,
    decimal Latitude,
    decimal Longitude
)
{
    public static AddressDto? FromOrDefault(Address? address)
    {
        if (address == null)
            return null;

        return From(address);
    }

    private static AddressDto From(Address address)
    {
        return new AddressDto(
            address.Id,
            address.Name, 
            address.GoogleLocationId, 
            address.Latitude, 
            address.Longitude
        );
    }

    public Address To(Guid universityId)
    {
        return new Address(
            Id ?? Guid.NewGuid(),
            universityId,
            Name,
            GoogleLocationId,
            Latitude,
            Longitude
        );
    }
}
