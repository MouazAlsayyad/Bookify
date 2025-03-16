using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings.CompleteBooking;

internal sealed class CompleteBookingCommandHandler : ICommandHandler<CompleteBookingCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingValidationService _validationService;

    public CompleteBookingCommandHandler(
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork,
        IBookingValidationService validationService)
    {
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _validationService = validationService;
    }

    public async Task<Result> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
    {
        Result<(Booking Booking, User User)> validationResult =
        await _validationService.ValidateAllAsync(request.BookingId, cancellationToken);

        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        (Booking booking, _) = validationResult.Value;

        // Complete the booking
        Result completeResult = booking.Complete(_dateTimeProvider.UtcNow);
        if (completeResult.IsFailure)
        {
            return completeResult;
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

