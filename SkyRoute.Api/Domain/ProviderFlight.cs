namespace SkyRoute.Api.Domain;

public sealed record ProviderFlight(
    string FlightNumber,
    DateTime DepartureAt,
    DateTime ArrivalAt,
    int DurationMinutes,
    CabinClass CabinClass,
    decimal BaseFare,
    string Currency);
