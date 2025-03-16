using Bookify.Domain.Apartments;

namespace Bookify.Api.Controllers.Apartments;

public sealed record ApartmentRequest(
    string Name,
    string Description,
    AddressRequest Address,
    decimal PriceAmount,
    decimal CleaningFeeAmount,
    string Currency, // Currency code (e.g., "USD", "EUR")
    List<Amenity> Amenities
);

public sealed record AddressRequest(
    string Country,
    string State,
    string ZipCode,
    string City,
    string Street
);
