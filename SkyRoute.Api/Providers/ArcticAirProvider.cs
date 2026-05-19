using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Providers;

public sealed class ArcticAirProvider : IFlightProvider
{
    public string Name => "ArcticAir";

    public Task<IReadOnlyCollection<ProviderFlight>> SearchAsync(
        FlightSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var flights = new[]
        {
            FlightMockData.CreateFlight(criteria, "AA", 0, new TimeOnly(7, 10), 8m, durationOptionOffset: 2),
            FlightMockData.CreateFlight(criteria, "AA", 1, new TimeOnly(12, 35), 18m, durationOptionOffset: 2),
            FlightMockData.CreateFlight(criteria, "AA", 2, new TimeOnly(18, 50), -6m, durationOptionOffset: 2)
        };

        return Task.FromResult<IReadOnlyCollection<ProviderFlight>>(flights);
    }
}
