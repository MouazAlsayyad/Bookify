using Bookify.Domain.Apartments;

namespace Bookify.Domain.Bookings;
public interface IPricingService
{
    PricingDetails CalculatePrice(Apartment apartment, DateRange period);
}
