using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings.ConfirmBooking;

internal sealed class ConfirmBookingCommandHandler : ICommandHandler<ConfirmBookingCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IBookingValidationService _validationService;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmBookingCommandHandler(
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork,
        IBookingValidationService validationService)
    {
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _validationService = validationService;
    }

    public async Task<Result> Handle(
        ConfirmBookingCommand request,
        CancellationToken cancellationToken)
    {
        Result<(Booking Booking, User User)> validationResult =
        await _validationService.ValidateAllAsync(request.BookingId, cancellationToken);

        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        (Booking booking, _) = validationResult.Value;


        // Confirm the booking
        Result result = booking.Confirm(_dateTimeProvider.UtcNow);

        if (result.IsFailure)
        {
            return result;
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
