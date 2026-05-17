namespace SkyRoute.Api.Domain;

public sealed record FlightOffer(
    string OfferId,
    string Provider,
    string FlightNumber,
    Airport Origin,
    Airport Destination,
    DateTime DepartureAt,
    DateTime ArrivalAt,
    int DurationMinutes,
    CabinClass CabinClass,
    decimal BaseFare,
    decimal PricePerPassenger,
    int PassengerCount,
    decimal TotalPrice,
    string Currency);
