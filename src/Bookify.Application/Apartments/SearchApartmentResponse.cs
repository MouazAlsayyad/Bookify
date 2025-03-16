using Bookify.Domain.Apartments;

namespace Bookify.Application.Apartments;

public sealed class SearchApartmentResponse
{
    private const int Zero = 0;

    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; }
    public AddressResponse Address { get; init; }
    public List<Amenity> Amenities { get; init; } = [];
    public double AverageRating { get; set; } = Zero;
    public int ReviewCount { get; set; } = Zero;
    public SearchApartmentResponse() { }
}
