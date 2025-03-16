using Bookify.Application.Abstractions.Caching;

namespace Bookify.Application.Apartments.SearchApartments;

public sealed record SearchApartmentsQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    int Page,
    int PageSize) : ICachedQuery<IReadOnlyList<SearchApartmentResponse>>
{
    public string CacheKey => $"apartments-{StartDate}-{EndDate}-{Page}-{PageSize}";

    public TimeSpan? Expiration => null;
}
