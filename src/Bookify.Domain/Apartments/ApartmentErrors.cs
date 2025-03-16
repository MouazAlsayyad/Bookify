using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

public static class ApartmentErrors
{
    public static readonly Error NotFound = new(
        "Apartment.NotFound",
        "The apartment with the specified identifier was not found");

    public static readonly Error Concurrency = new(
       "Apartment.Concurrency",
       "A concurrency conflict occurred while saving the apartment.");

    public static readonly Error Invalid = new(
       "Apartment.Invalid ",
       "The apartment data is invalid. Please check the provided details and try again."
       );
}
