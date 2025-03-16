using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;
using FluentValidation;

namespace Bookify.Application.Apartments.CreateApartment;

internal class CreateApartmentCommandValidator : AbstractValidator<CreateApartmentCommand>
{
    public CreateApartmentCommandValidator()
    {
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

// Validator for the Name record
public class NameValidator : AbstractValidator<Name>
{
    public NameValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Name value is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");
    }
}

// Validator for the Description record
public class DescriptionValidator : AbstractValidator<Description>
{
    public DescriptionValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Description value is required.")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");
    }
}

// Validator for the Address record
public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required.")
            .MaximumLength(50)
            .WithMessage("Country must not exceed 50 characters.");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required.")
            .MaximumLength(50)
            .WithMessage("State must not exceed 50 characters.");

        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage("Zip code is required.")
            .MaximumLength(20)
            .WithMessage("Zip code must not exceed 20 characters.");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required.")
            .MaximumLength(50)
            .WithMessage("City must not exceed 50 characters.");

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street is required.")
            .MaximumLength(100)
            .WithMessage("Street must not exceed 100 characters.");
    }
}

// Validator for the Money record
public class MoneyValidator : AbstractValidator<Money>
{
    public MoneyValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Amount must be greater than or equal to 0.");

        RuleFor(x => x.Currency)
            .NotNull()
            .WithMessage("Currency is required.")
            .Must(currency => currency != null && Currency.All.Contains(currency))
            .WithMessage("Currency must be valid.");
    }
}
