using SkyRoute.Api.Pricing;

namespace SkyRoute.Api.Tests;

public sealed class PricingTests
{
    [Fact]
    public void GlobalAir_adds_fuel_surcharge_and_rounds_to_two_decimals()
    {
        var strategy = new GlobalAirPricingStrategy();

        var price = strategy.CalculatePricePerPassenger(100.555m);

        Assert.Equal(115.64m, price);
    }

    [Fact]
    public void BudgetWings_applies_discount()
    {
        var strategy = new BudgetWingsPricingStrategy();

        var price = strategy.CalculatePricePerPassenger(100m);

        Assert.Equal(90m, price);
    }

    [Fact]
    public void BudgetWings_never_goes_below_minimum_price()
    {
        var strategy = new BudgetWingsPricingStrategy();

        var price = strategy.CalculatePricePerPassenger(10m);

        Assert.Equal(29.99m, price);
    }
}
