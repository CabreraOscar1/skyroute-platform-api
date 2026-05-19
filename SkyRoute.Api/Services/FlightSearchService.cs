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
    private readonly IReadOnlyDictionary<string, IPricingStrategy> _pricingStrategies;

    public FlightSearchService(
        IAirportCatalog airportCatalog,
        IOfferStore offerStore,
        IEnumerable<IFlightProvider> providers,
        IEnumerable<IPricingStrategy> pricingStrategies)
    {
        _airportCatalog = airportCatalog;
        _offerStore = offerStore;
        _providers = providers.ToArray();
        _pricingStrategies = pricingStrategies.ToDictionary(
            strategy => strategy.ProviderName,
            StringComparer.OrdinalIgnoreCase);
    }

    public async Task<FlightSearchResult> SearchAsync(
        FlightSearchRequest request,
        CancellationToken cancellationToken)
    {
        var criteria = ValidateAndCreateCriteria(request);
        var searchToken = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var searchId = $"SRCH-{searchToken}";
        var offers = new List<FlightOffer>();

        foreach (var provider in _providers)
        {
            var providerFlights = await provider.SearchAsync(criteria, cancellationToken);
            var pricingStrategy = GetPricingStrategy(provider.Name);

            var providerOffers = providerFlights.Select((flight, index) =>
                CreateOffer(searchToken, provider.Name, flight, criteria, pricingStrategy, index));

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
            errors[nameof(request.Origin)] = ["Origin airport is not supported. Use one of the airports returned by GET /api/airports."];
        }

        if (string.IsNullOrWhiteSpace(request.Destination))
        {
            errors[nameof(request.Destination)] = ["Destination is required."];
        }
        else if (destination is null)
        {
            errors[nameof(request.Destination)] = ["Destination airport is not supported. Use one of the airports returned by GET /api/airports."];
        }

        if (origin is not null &&
            destination is not null &&
            origin.Code == destination.Code)
        {
            errors[nameof(request.Destination)] = ["Origin and destination must be different airports."];
        }

        if (request.DepartureDate == default)
        {
            errors[nameof(request.DepartureDate)] = ["Departure date is required."];
        }
        else if (request.DepartureDate < DateOnly.FromDateTime(DateTime.Today))
        {
            errors[nameof(request.DepartureDate)] = ["Departure date must be today or a future date."];
        }

        if (request.PassengerCount is < 1 or > 9)
        {
            errors[nameof(request.PassengerCount)] = ["Passenger count must be between 1 and 9."];
        }

        if (!Enum.IsDefined(request.CabinClass))
        {
            errors[nameof(request.CabinClass)] = ["Cabin class must be Economy, Business, or FirstClass."];
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

    private IPricingStrategy GetPricingStrategy(string providerName)
    {
        if (_pricingStrategies.TryGetValue(providerName, out var pricingStrategy))
        {
            return pricingStrategy;
        }

        throw new InvalidOperationException($"No pricing strategy registered for provider '{providerName}'.");
    }

    private static FlightOffer CreateOffer(
        string searchToken,
        string providerName,
        ProviderFlight flight,
        FlightSearchCriteria criteria,
        IPricingStrategy pricingStrategy,
        int index)
    {
        var pricePerPassenger = pricingStrategy.CalculatePricePerPassenger(flight.BaseFare);
        var totalPrice = Math.Round(pricePerPassenger * criteria.PassengerCount, 2);
        var providerCode = CreateProviderCode(providerName);
        var offerId = $"OFF-{providerCode}-{searchToken}-{index + 1:D2}";

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
            "GlobalAir" => "GA",
            "BudgetWings" => "BW",
            "ArcticAir" => "AA",
            _ => providerName[..Math.Min(2, providerName.Length)].ToUpperInvariant()
        };
}
