using System.Data;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Dapper;

namespace Bookify.Application.Apartments.GetApartment;

internal sealed class GetApartmentQueryHandler : IQueryHandler<GetApartmentQuery, ApartmentResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetApartmentQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<ApartmentResponse>> Handle(GetApartmentQuery request, CancellationToken cancellationToken)
    {
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
                r.id AS ReviewId,
                r.user_id AS UserId,
                u.first_name AS FirstName,
                u.last_name AS LastName,
                r.rating AS Rating,
                r.comment AS Comment,
                r.created_on_utc AS CreatedOnUtc
            FROM apartments AS a
            LEFT JOIN reviews AS r ON a.id = r.apartment_id
            LEFT JOIN users AS u ON r.user_id = u.id 
            WHERE a.id = @ApartmentId
        """;

        var apartmentDictionary = new Dictionary<Guid, ApartmentResponse>();

        await connection.QueryAsync<ApartmentResponse, AddressResponse, int[], ReviewResponse, ApartmentResponse>(
        sql,
        (apartment, address, amenities, review) =>
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
                    Reviews = []
                };
                apartmentDictionary.Add(apartmentEntry.Id, apartmentEntry);
            }

            if (review.FirstName != null)
            {
                apartmentEntry.Reviews.Add(review);
            }

            return apartmentEntry;
        },
        new { request.ApartmentId },
        splitOn: "Country,Amenities,ReviewId");

        ApartmentResponse? apartmentResult = apartmentDictionary.Values.FirstOrDefault();

        if (apartmentResult is null)
        {
            return Result.Failure<ApartmentResponse>(ApartmentErrors.NotFound);
        }

        // Calculate the average rating
        if (apartmentResult.Reviews.Any())
        {
            apartmentResult.AverageRating = apartmentResult.Reviews.Average(r => r.Rating);
            apartmentResult.ReviewCount = apartmentResult.Reviews.Count;
        }


        return Result.Success(apartmentResult);
    }
}
