using SkyRoute.Api.Contracts;
using SkyRoute.Api.Domain;
using SkyRoute.Api.Pricing;
using SkyRoute.Api.Providers;
using SkyRoute.Api.Services;
using SkyRoute.Api.Validation;

namespace SkyRoute.Api.Tests;

public sealed class FlightSearchServiceTests
{
    [Fact]
    public async Task Search_returns_normalized_results_from_both_providers()
    {
        var store = new InMemoryOfferStore();
        var service = CreateService(store);
        var request = new FlightSearchRequest("EZE", "MIA", FutureDepartureDate(), 2, CabinClass.Economy);

        var result = await service.SearchAsync(request, CancellationToken.None);

        Assert.StartsWith("SRCH-", result.SearchId);
        Assert.Equal(6, result.Results.Count);
        Assert.Contains(result.Results, offer => offer.Provider == "GlobalAir");
        Assert.Contains(result.Results, offer => offer.Provider == "BudgetWings");
        Assert.All(result.Results, offer =>
        {
            Assert.StartsWith("OFF-", offer.OfferId);
            Assert.Equal(Math.Round(offer.PricePerPassenger * request.PassengerCount, 2), offer.TotalPrice);
            Assert.NotNull(store.FindById(offer.OfferId));
        });
    }

    [Theory]
    [InlineData("EZE", "EZE", 2)]
    [InlineData("EZE", "MIA", 0)]
    [InlineData("EZE", "MIA", 10)]
    public async Task Search_rejects_invalid_criteria(string origin, string destination, int passengers)
    {
        var service = CreateService(new InMemoryOfferStore());
        var request = new FlightSearchRequest(origin, destination, FutureDepartureDate(), passengers, CabinClass.Economy);

        await Assert.ThrowsAsync<FlightSearchValidationException>(() =>
            service.SearchAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task Search_rejects_past_departure_date()
    {
        var service = CreateService(new InMemoryOfferStore());
        var pastDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var request = new FlightSearchRequest("EZE", "MIA", pastDate, 2, CabinClass.Economy);

        await Assert.ThrowsAsync<FlightSearchValidationException>(() =>
            service.SearchAsync(request, CancellationToken.None));
    }

    private static DateOnly FutureDepartureDate() =>
        DateOnly.FromDateTime(DateTime.Today.AddDays(30));

    private static FlightSearchService CreateService(IOfferStore offerStore) =>
        new(
            new AirportCatalog(),
            offerStore,
            [new GlobalAirProvider(), new BudgetWingsProvider()],
            [new GlobalAirPricingStrategy(), new BudgetWingsPricingStrategy()]);
}
