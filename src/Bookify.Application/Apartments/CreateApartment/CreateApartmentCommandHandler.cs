﻿using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using FluentValidation.Results;

namespace Bookify.Application.Apartments.CreateApartment;
internal class CreateApartmentCommandHandler : ICommandHandler<CreateApartmentCommand, Guid>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateApartmentCommandValidator _validator;

    public CreateApartmentCommandHandler(IApartmentRepository apartmentRepository, IUnitOfWork unitOfWork, CreateApartmentCommandValidator validator)
    {
        _apartmentRepository = apartmentRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Guid>> Handle(CreateApartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(ApartmentErrors.Invalid);
            }

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
