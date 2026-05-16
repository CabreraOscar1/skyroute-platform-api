using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Contracts;

public sealed record FlightOfferResponse(
    string OfferId,
    string Provider,
    string FlightNumber,
    AirportResponse Origin,
    AirportResponse Destination,
    DateTime DepartureAt,
    DateTime ArrivalAt,
    int DurationMinutes,
    CabinClass CabinClass,
    string Currency,
    decimal PricePerPassenger,
    int PassengerCount,
    decimal TotalPrice)
{
    public static FlightOfferResponse FromOffer(FlightOffer offer) =>
        new(
            offer.OfferId,
            offer.Provider,
            offer.FlightNumber,
            AirportResponse.FromAirport(offer.Origin),
            AirportResponse.FromAirport(offer.Destination),
            offer.DepartureAt,
            offer.ArrivalAt,
            offer.DurationMinutes,
            offer.CabinClass,
            offer.Currency,
            offer.PricePerPassenger,
            offer.PassengerCount,
            offer.TotalPrice);
}
