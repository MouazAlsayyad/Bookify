using System.Data;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Dapper;

namespace Bookify.Application.Apartments.SearchApartments;

internal sealed class SearchApartmentsQueryHandler
    : IQueryHandler<SearchApartmentsQuery, IReadOnlyList<ApartmentResponse>>
{
    private static readonly int[] ActiveBookingStatuses =
    {
        (int)BookingStatus.Reserved,
        (int)BookingStatus.Confirmed,
        (int)BookingStatus.Completed
    };

    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly SearchApartmentsQueryValidator _validator;

    public SearchApartmentsQueryHandler(ISqlConnectionFactory sqlConnectionFactory, SearchApartmentsQueryValidator validator)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _validator = validator;
    }

    public async Task<Result<IReadOnlyList<ApartmentResponse>>> Handle(SearchApartmentsQuery request, CancellationToken cancellationToken)
    {
        FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<IReadOnlyList<ApartmentResponse>>(
               ApartmentErrors.Invalid);
        }

        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
        SELECT
            a.id AS Id,
            a.name AS Name,
            a.description AS Description,
            a.price_amount AS Price,
            a.price_currency AS Currency,
            a.address_country AS Country,
            a.address_state AS State,
            a.address_zip_code AS ZipCode,
            a.address_city AS City,
            a.address_street AS Street,
            a.amenities AS Amenities,
            r.rating AS Rating
        FROM apartments AS a
        LEFT JOIN reviews AS r ON a.id = r.apartment_id
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM bookings AS b
            WHERE
                b.apartment_id = a.id AND
                b.duration_start <= @EndDate AND
                b.duration_end >= @StartDate AND
                b.status = ANY(@ActiveBookingStatuses)
        )
        ORDER BY a.id
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY
        """;

        var apartmentDictionary = new Dictionary<Guid, ApartmentResponse>();

        int offset = (request.Page - 1) * request.PageSize;
        int pageSize = request.PageSize;

        await connection.QueryAsync<ApartmentResponse, AddressResponse, int[], int?, ApartmentResponse>(
           sql,
           (apartment, address, amenities, rating) =>
           {
               if (!apartmentDictionary.TryGetValue(apartment.Id, out ApartmentResponse? apartmentEntry))
               {
                   apartmentEntry = new ApartmentResponse
                   {
                       Id = apartment.Id,
                       Name = apartment.Name,
                       Description = apartment.Description,
                       Price = apartment.Price,
                       Currency = apartment.Currency,
                       Address = address,
                       Amenities = amenities.Select(a => (Amenity)a).ToList(),
                       AverageRating = 0 // Initialize average rating
                   };
                   apartmentDictionary.Add(apartmentEntry.Id, apartmentEntry);
               }

               // Collect ratings for the apartment
               if (rating.HasValue)
               {
                   apartmentEntry.AverageRating = (apartmentEntry.AverageRating
                   * apartmentEntry.ReviewCount
                   + rating.Value) / (apartmentEntry.ReviewCount + 1);
                   apartmentEntry.ReviewCount++;
               }

               return apartmentEntry;
           },
           new
           {
               request.StartDate,
               request.EndDate,
               ActiveBookingStatuses,
               Offset = offset,
               PageSize = pageSize
           },
           splitOn: "Country,Amenities,Rating");

        // Convert the dictionary values to a list
        var apartments = apartmentDictionary.Values.ToList();

        return apartments;
    }
}
