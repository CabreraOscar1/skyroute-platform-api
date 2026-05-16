using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Providers;

public sealed class BudgetWingsProvider : IFlightProvider
{
    public string Name => "BudgetWings";

    public Task<IReadOnlyCollection<ProviderFlight>> SearchAsync(
        FlightSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var flights = new[]
        {
            CreateFlight(criteria, "BW", 0, new TimeOnly(6, 45), -18m),
            CreateFlight(criteria, "BW", 1, new TimeOnly(11, 20), -5m),
            CreateFlight(criteria, "BW", 2, new TimeOnly(16, 55), 16m)
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
        var durationMinutes = FlightMockData.CalculateDurationMinutes(criteria, option + 1);
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
