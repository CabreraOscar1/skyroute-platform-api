using SkyRoute.Api.Contracts;
using SkyRoute.Api.Domain;
using SkyRoute.Api.Pricing;
using SkyRoute.Api.Providers;
using SkyRoute.Api.Validation;

namespace SkyRoute.Api.Services;

public sealed class FlightSearchService : IFlightSearchService
{
    private readonly IAirportCatalog _airportCatalog;
    private readonly IOfferStore _offerStore;
    private readonly IReadOnlyCollection<IFlightProvider> _providers;
    private readonly IReadOnlyCollection<IPricingStrategy> _pricingStrategies;

    public FlightSearchService(
        IAirportCatalog airportCatalog,
        IOfferStore offerStore,
        IEnumerable<IFlightProvider> providers,
        IEnumerable<IPricingStrategy> pricingStrategies)
    {
        _airportCatalog = airportCatalog;
        _offerStore = offerStore;
        _providers = providers.ToArray();
        _pricingStrategies = pricingStrategies.ToArray();
    }

    public async Task<FlightSearchResult> SearchAsync(
        FlightSearchRequest request,
        CancellationToken cancellationToken)
    {
        var criteria = ValidateAndCreateCriteria(request);
        var searchId = $"srch_{Guid.NewGuid():N}"[..13];
        var offers = new List<FlightOffer>();

        foreach (var provider in _providers)
        {
            var providerFlights = await provider.SearchAsync(criteria, cancellationToken);
            var pricingStrategy = GetPricingStrategy(provider.Name);

            var providerOffers = providerFlights.Select((flight, index) =>
                CreateOffer(searchId, provider.Name, flight, criteria, pricingStrategy, index));

            offers.AddRange(providerOffers);
        }

        _offerStore.SaveMany(offers);

        return new FlightSearchResult(searchId, criteria, offers);
    }

    private FlightSearchCriteria ValidateAndCreateCriteria(FlightSearchRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        var origin = _airportCatalog.FindByCode(request.Origin);
        var destination = _airportCatalog.FindByCode(request.Destination);

        if (string.IsNullOrWhiteSpace(request.Origin))
        {
            errors[nameof(request.Origin)] = ["Origin is required."];
        }
        else if (origin is null)
        {
            errors[nameof(request.Origin)] = ["Origin airport is not supported."];
        }

        if (string.IsNullOrWhiteSpace(request.Destination))
        {
            errors[nameof(request.Destination)] = ["Destination is required."];
        }
        else if (destination is null)
        {
            errors[nameof(request.Destination)] = ["Destination airport is not supported."];
        }

        if (origin is not null &&
            destination is not null &&
            origin.Code == destination.Code)
        {
            errors[nameof(request.Destination)] = ["Origin and destination must be different."];
        }

        if (request.DepartureDate == default)
        {
            errors[nameof(request.DepartureDate)] = ["Departure date is required."];
        }

        if (request.PassengerCount is < 1 or > 9)
        {
            errors[nameof(request.PassengerCount)] = ["Passenger count must be between 1 and 9."];
        }

        if (!Enum.IsDefined(request.CabinClass))
        {
            errors[nameof(request.CabinClass)] = ["Cabin class is not supported."];
        }

        if (errors.Count > 0 || origin is null || destination is null)
        {
            throw new FlightSearchValidationException(errors);
        }

        return new FlightSearchCriteria(
            origin,
            destination,
            request.DepartureDate,
            request.PassengerCount,
            request.CabinClass);
    }

    private IPricingStrategy GetPricingStrategy(string providerName) =>
        _pricingStrategies.FirstOrDefault(strategy =>
            string.Equals(strategy.ProviderName, providerName, StringComparison.OrdinalIgnoreCase))
        ?? throw new InvalidOperationException($"No pricing strategy registered for provider '{providerName}'.");

    private static FlightOffer CreateOffer(
        string searchId,
        string providerName,
        ProviderFlight flight,
        FlightSearchCriteria criteria,
        IPricingStrategy pricingStrategy,
        int index)
    {
        var pricePerPassenger = pricingStrategy.CalculatePricePerPassenger(flight.BaseFare);
        var totalPrice = Math.Round(pricePerPassenger * criteria.PassengerCount, 2);
        var providerCode = CreateProviderCode(providerName);
        var offerId = $"off_{providerCode}_{searchId[5..]}_{index + 1}";

        return new FlightOffer(
            offerId,
            providerName,
            flight.FlightNumber,
            criteria.Origin,
            criteria.Destination,
            flight.DepartureAt,
            flight.ArrivalAt,
            flight.DurationMinutes,
            flight.CabinClass,
            flight.BaseFare,
            pricePerPassenger,
            criteria.PassengerCount,
            totalPrice,
            flight.Currency);
    }

    private static string CreateProviderCode(string providerName) =>
        providerName switch
        {
            "GlobalAir" => "ga",
            "BudgetWings" => "bw",
            _ => providerName[..Math.Min(2, providerName.Length)].ToLowerInvariant()
        };
}
