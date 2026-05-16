using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Services;

public sealed class AirportCatalog : IAirportCatalog
{
    private static readonly Airport[] Airports =
    [
        new("EZE", "Ministro Pistarini International Airport", "Buenos Aires", "AR"),
        new("AEP", "Jorge Newbery Airfield", "Buenos Aires", "AR"),
        new("COR", "Ingeniero Aeronautico Ambrosio L.V. Taravella International Airport", "Cordoba", "AR"),
        new("MIA", "Miami International Airport", "Miami", "US"),
        new("JFK", "John F. Kennedy International Airport", "New York", "US"),
        new("LAX", "Los Angeles International Airport", "Los Angeles", "US")
    ];

    public IReadOnlyCollection<Airport> GetAll() => Airports;

    public Airport? FindByCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return Airports.FirstOrDefault(airport =>
            string.Equals(airport.Code, code.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
