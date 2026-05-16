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
            CreateFlight(criteria, "GA", 0, new TimeOnly(8, 15), 0m),
            CreateFlight(criteria, "GA", 1, new TimeOnly(13, 40), 28m),
            CreateFlight(criteria, "GA", 2, new TimeOnly(19, 5), -12m)
        };

        return Task.FromResult<IReadOnlyCollection<ProviderFlight>>(flights);
    }

    private static ProviderFlight CreateFlight(
        FlightSearchCriteria criteria,
        string prefix,
        int option,
        TimeOnly departureTime,
        decimal fareAdjustment)
    {
        var departureAt = criteria.DepartureDate.ToDateTime(departureTime);
        var durationMinutes = FlightMockData.CalculateDurationMinutes(criteria, option);
        var arrivalAt = departureAt.AddMinutes(durationMinutes);
        var baseFare = FlightMockData.CalculateBaseFare(criteria, option, fareAdjustment);
        var flightNumber = FlightMockData.CreateFlightNumber(prefix, criteria, option);

        return new ProviderFlight(
            flightNumber,
            departureAt,
            arrivalAt,
            durationMinutes,
            criteria.CabinClass,
            baseFare,
            "USD");
    }
}
