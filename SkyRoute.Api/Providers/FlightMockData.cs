using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Providers;

internal static class FlightMockData
{
    public static ProviderFlight CreateFlight(
        FlightSearchCriteria criteria,
        string providerPrefix,
        int option,
        TimeOnly departureTime,
        decimal fareAdjustment,
        int durationOptionOffset = 0)
    {
        var departureAt = criteria.DepartureDate.ToDateTime(departureTime);
        var durationMinutes = CalculateDurationMinutes(criteria, option + durationOptionOffset);
        var arrivalAt = departureAt.AddMinutes(durationMinutes);
        var baseFare = CalculateBaseFare(criteria, option, fareAdjustment);
        var flightNumber = CreateFlightNumber(providerPrefix, criteria, option);

        return new ProviderFlight(
            flightNumber,
            departureAt,
            arrivalAt,
            durationMinutes,
            criteria.CabinClass,
            baseFare,
            "USD");
    }

    public static int CalculateDurationMinutes(FlightSearchCriteria criteria, int option)
    {
        var isDomestic = criteria.Origin.CountryCode == criteria.Destination.CountryCode;
        var baseDuration = isDomestic ? 95 : 485;
        var routeOffset = StableRouteSeed(criteria) % (isDomestic ? 80 : 140);

        return baseDuration + routeOffset + (option * 35);
    }

    private static decimal CalculateBaseFare(
        FlightSearchCriteria criteria,
        int option,
        decimal fareAdjustment)
    {
        var isDomestic = criteria.Origin.CountryCode == criteria.Destination.CountryCode;
        var baseFare = isDomestic ? 86m : 265m;
        var routeOffset = StableRouteSeed(criteria) % (isDomestic ? 65 : 180);
        var cabinMultiplier = criteria.CabinClass switch
        {
            CabinClass.Economy => 1.00m,
            CabinClass.Business => 1.85m,
            CabinClass.FirstClass => 2.70m,
            _ => 1.00m
        };

        return Math.Round(((baseFare + routeOffset + (option * 24m)) * cabinMultiplier) + fareAdjustment, 2);
    }

    private static string CreateFlightNumber(
        string prefix,
        FlightSearchCriteria criteria,
        int option)
    {
        var number = 100 + ((StableRouteSeed(criteria) + (option * 137)) % 800);
        return $"{prefix}{number}";
    }

    private static int StableRouteSeed(FlightSearchCriteria criteria)
    {
        var route = $"{criteria.Origin.Code}{criteria.Destination.Code}";
        return route.Sum(character => character);
    }
}
