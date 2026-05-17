using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Providers;

public sealed class GlobalAirProvider : IFlightProvider
{
    public string Name => "GlobalAir";

    public Task<IReadOnlyCollection<ProviderFlight>> SearchAsync(
        FlightSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var flights = new[]
        {
            FlightMockData.CreateFlight(criteria, "GA", 0, new TimeOnly(8, 15), 0m),
            FlightMockData.CreateFlight(criteria, "GA", 1, new TimeOnly(13, 40), 28m),
            FlightMockData.CreateFlight(criteria, "GA", 2, new TimeOnly(19, 5), -12m)
        };

        return Task.FromResult<IReadOnlyCollection<ProviderFlight>>(flights);
    }
}
