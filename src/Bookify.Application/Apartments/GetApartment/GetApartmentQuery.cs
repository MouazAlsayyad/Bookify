﻿using Bookify.Application.Abstractions.Caching;

namespace Bookify.Application.Apartments.GetApartment;
public sealed record GetApartmentQuery(Guid ApartmentId) : ICachedQuery<ApartmentResponse>
{
    public string CacheKey => $"apartments-{ApartmentId}";

    public TimeSpan? Expiration => null;
}
