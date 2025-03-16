using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Application.Apartments.CreateApartment;
public sealed record CreateApartmentCommand(
    Name Name,
    Description Description,
    Address Address,
    Money Price,
    Money CleaningFee,
    List<Amenity> Amenities
) : ICommand<Guid>;
