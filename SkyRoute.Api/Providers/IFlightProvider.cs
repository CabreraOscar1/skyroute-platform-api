using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Providers;

public interface IFlightProvider
{
    string Name { get; }

    Task<IReadOnlyCollection<ProviderFlight>> SearchAsync(
        FlightSearchCriteria criteria,
        CancellationToken cancellationToken);
}
