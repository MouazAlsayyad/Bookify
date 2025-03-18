using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using FluentValidation.Results;

namespace Bookify.Application.Apartments.UpdateApartment;
internal class UpdateApartmentCommandHandler : ICommandHandler<UpdateApartmentCommand, Guid>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateApartmentCommandValidator _validator;
    public UpdateApartmentCommandHandler(IApartmentRepository apartmentRepository, IUnitOfWork unitOfWork, UpdateApartmentCommandValidator validator)
    {
        _apartmentRepository = apartmentRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Guid>> Handle(UpdateApartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(ApartmentErrors.Invalid);
            }

            Apartment? apartment = await _apartmentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (apartment is null)
            {
                return Result.Failure<Guid>(ApartmentErrors.NotFound);
            }

            apartment.Update(
                request.Name,
                request.Description,
                request.Address,
                request.Price,
                request.CleaningFee,
                request.Amenities
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return apartment.Id;
        }
        catch (ConcurrencyException)
        {
            return Result.Failure<Guid>(ApartmentErrors.Concurrency);
        }
    }
}
