using Bookify.Application.Apartments.CreateApartment;
using FluentValidation;

namespace Bookify.Application.Apartments.UpdateApartment;
internal class UpdateApartmentCommandValidator : AbstractValidator<UpdateApartmentCommand>
{
    public UpdateApartmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Apartment ID is required.");

        RuleFor(x => x.Name)
            .NotNull()
            .WithMessage("Name is required.")
            .SetValidator(new NameValidator());

        RuleFor(x => x.Description)
            .NotNull()
            .WithMessage("Description is required.")
            .SetValidator(new DescriptionValidator());

        RuleFor(x => x.Address)
            .NotNull()
            .WithMessage("Address is required.")
            .SetValidator(new AddressValidator());

        RuleFor(x => x.Price)
            .NotNull()
            .WithMessage("Price is required.")
            .SetValidator(new MoneyValidator());

        RuleFor(x => x.CleaningFee)
            .NotNull()
            .WithMessage("Cleaning fee is required.")
            .SetValidator(new MoneyValidator());

        RuleFor(x => x.Amenities)
            .NotNull()
            .WithMessage("Amenities are required.")
            .Must(amenities => amenities != null && amenities.Any())
            .WithMessage("At least one amenity is required.");
    }
}
