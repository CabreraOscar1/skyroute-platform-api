using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Providers
{
    public sealed class ArcticAirProvider : IFlightProvider
    {
        public string Name => "ArcticAirProvider";

        public Task<IReadOnlyCollection<ProviderFlight>> SearchAsync(
            FlightSearchCriteria criteria,
            CancellationToken cancellationToken)
        {
            var flights = new[]
            {
            FlightMockData.CreateFlight(criteria, "BW", 0, new TimeOnly(6, 45), -18m, durationOptionOffset: 1),
            FlightMockData.CreateFlight(criteria, "BW", 1, new TimeOnly(11, 20), -5m, durationOptionOffset: 1),
            FlightMockData.CreateFlight(criteria, "BW", 2, new TimeOnly(16, 55), 16m, durationOptionOffset: 1)
        };

            return Task.FromResult<IReadOnlyCollection<ProviderFlight>>(flights);
        }
    }
}
