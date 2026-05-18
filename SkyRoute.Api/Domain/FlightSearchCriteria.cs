namespace SkyRoute.Api.Domain;

public sealed record FlightSearchCriteria(
    Airport Origin,
    Airport Destination,
    DateOnly DepartureDate,
    int PassengerCount,
    CabinClass CabinClass);
