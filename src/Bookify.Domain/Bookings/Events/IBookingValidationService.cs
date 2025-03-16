using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;

namespace Bookify.Domain.Bookings.Events;
public interface IBookingValidationService
{
    Task<Result<(Booking Booking, User User)>> ValidateAllAsync(Guid bookingId, CancellationToken cancellationToken);
    Task<Result<Booking>> ValidateBookingAsync(Guid bookingId, CancellationToken cancellationToken);
    Task<Result<User>> ValidateUserAsync();
    Result ValidateBookingOwnership(Booking booking, User user);
}
