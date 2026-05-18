using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Contracts;

public sealed record FlightSearchRequest(
    string? Origin,
    string? Destination,
    DateOnly DepartureDate,
    int PassengerCount,
    CabinClass CabinClass);
