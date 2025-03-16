using Bookify.Application.Abstractions.Authentication;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings;
public class BookingValidationService : IBookingValidationService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public BookingValidationService(
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        IUserContext userContext)
    {
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<Result<(Booking Booking, User User)>> ValidateAllAsync(Guid bookingId, CancellationToken cancellationToken)
    {
        // Validate booking
        Result<Booking> bookingResult = await ValidateBookingAsync(bookingId, cancellationToken);
        if (bookingResult.IsFailure)
        {
            return Result.Failure<(Booking, User)>(bookingResult.Error);
        }

        // Validate user
        Result<User> userResult = await ValidateUserAsync();
        if (userResult.IsFailure)
        {
            return Result.Failure<(Booking, User)>(userResult.Error);
        }

        // Validate booking ownership
        Booking booking = bookingResult.Value;
        User user = userResult.Value;

        Result ownershipResult = ValidateBookingOwnership(booking, user);
        if (ownershipResult.IsFailure)
        {
            return Result.Failure<(Booking, User)>(ownershipResult.Error);
        }

        return Result.Success((booking, user));
    }


    public async Task<Result<Booking>> ValidateBookingAsync(Guid bookingId, CancellationToken cancellationToken)
    {
        Booking? booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null)
        {
            return Result.Failure<Booking>(BookingErrors.NotFound);
        }

        return Result.Success(booking);
    }

    public async Task<Result<User>> ValidateUserAsync()
    {
        User? user = await _userRepository.GetUserByIdentityIdAsync(_userContext.IdentityId);
        if (user is null)
        {
            return Result.Failure<User>(UserErrors.NotFound);
        }

        return Result.Success(user);
    }

    public Result ValidateBookingOwnership(Booking booking, User user)
    {
        if (booking.UserId != user.Id)
        {
            return Result.Failure(BookingErrors.NotAuthorized);
        }

        return Result.Success();
    }

}
