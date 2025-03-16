using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;

namespace Bookify.Application.Apartments.CreateApartment;
internal class CreateApartmentCommandHandler : ICommandHandler<CreateApartmentCommand, Guid>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateApartmentCommandHandler(IApartmentRepository apartmentRepository, IUnitOfWork unitOfWork)
    {
        _apartmentRepository = apartmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateApartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var apartment = Apartment.Create(
                request.Name,
                request.Description,
                request.Address,
                request.Price,
                request.CleaningFee,
                request.Amenities
           );

            await _apartmentRepository.AddAsync(apartment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return apartment.Id;
        }
        catch (ConcurrencyException)
        {
            return Result.Failure<Guid>(ApartmentErrors.Concurrency);
        }
    }
}
