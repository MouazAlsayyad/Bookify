using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings.CancelBooking;

internal sealed class CancelBookingCommandHandler : ICommandHandler<CancelBookingCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingValidationService _validationService;

    public CancelBookingCommandHandler(
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork,
        IBookingValidationService validationService)
    {
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _validationService = validationService;
    }

    public async Task<Result> Handle(
        CancelBookingCommand request,
        CancellationToken cancellationToken)
    {
        Result<(Booking Booking, User User)> validationResult =
        await _validationService.ValidateAllAsync(request.BookingId, cancellationToken);

        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        (Booking booking, _) = validationResult.Value;

        // Cancel the booking
        Result result = booking.Cancel(_dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            return result;
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
