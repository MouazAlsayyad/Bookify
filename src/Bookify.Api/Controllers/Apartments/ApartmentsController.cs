using Asp.Versioning;
using Bookify.Application.Apartments;
using Bookify.Application.Apartments.CreateApartment;
using Bookify.Application.Apartments.GetApartment;
using Bookify.Application.Apartments.SearchApartments;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Apartments;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/apartments")]
public sealed class ApartmentsController : ControllerBase
{
    private readonly ISender _sender;

    public ApartmentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> SearchApartments(
        DateOnly startDate,
        DateOnly endDate,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchApartmentsQuery(startDate, endDate, page, pageSize);

        Result<IReadOnlyList<ApartmentResponse>> result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetApartment(
       Guid id,
       CancellationToken cancellationToken)
    {
        var query = new GetApartmentQuery(id);

        Result<ApartmentResponse> result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateApartment(ApartmentRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateApartmentCommand(
            Name: new Name(request.Name),
            Description: new Description(request.Description),
            Address: new Address(
                request.Address.Country,
                request.Address.State,
                request.Address.ZipCode,
                request.Address.City,
                request.Address.Street
            ),
            Price: new Money(request.PriceAmount, Currency.FromCode(request.Currency)),
            CleaningFee: new Money(request.CleaningFeeAmount, Currency.FromCode(request.Currency)),
            Amenities: request.Amenities
        );

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetApartment), new { id = result.Value }, result.Value);
    }
}
