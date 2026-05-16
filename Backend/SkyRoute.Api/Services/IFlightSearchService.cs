using SkyRoute.Api.Contracts;
using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Services;

public interface IFlightSearchService
{
    Task<FlightSearchResult> SearchAsync(
        FlightSearchRequest request,
        CancellationToken cancellationToken);
}
