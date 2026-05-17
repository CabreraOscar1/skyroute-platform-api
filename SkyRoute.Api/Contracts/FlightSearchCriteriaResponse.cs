using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Contracts;

public sealed record FlightSearchCriteriaResponse(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    int PassengerCount,
    CabinClass CabinClass)
{
    public static FlightSearchCriteriaResponse FromCriteria(FlightSearchCriteria criteria) =>
        new(
            criteria.Origin.Code,
            criteria.Destination.Code,
            criteria.DepartureDate,
            criteria.PassengerCount,
            criteria.CabinClass);
}
