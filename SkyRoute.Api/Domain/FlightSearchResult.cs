namespace SkyRoute.Api.Domain;

public sealed record FlightSearchResult(
    string SearchId,
    FlightSearchCriteria Criteria,
    IReadOnlyCollection<FlightOffer> Results);
