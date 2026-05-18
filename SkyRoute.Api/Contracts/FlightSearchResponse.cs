using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Contracts;

public sealed record FlightSearchResponse(
    string SearchId,
    FlightSearchCriteriaResponse Criteria,
    IReadOnlyCollection<FlightOfferResponse> Results)
{
    public static FlightSearchResponse FromResult(FlightSearchResult result) =>
        new(
            result.SearchId,
            FlightSearchCriteriaResponse.FromCriteria(result.Criteria),
            result.Results.Select(FlightOfferResponse.FromOffer).ToArray());
}
