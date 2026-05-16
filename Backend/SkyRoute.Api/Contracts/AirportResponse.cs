using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Contracts;

public sealed record AirportResponse(
    string Code,
    string Name,
    string City,
    string CountryCode)
{
    public static AirportResponse FromAirport(Airport airport) =>
        new(airport.Code, airport.Name, airport.City, airport.CountryCode);
}
