namespace SkyRoute.Api.Pricing;

public sealed class ArcticAirPricingStrategy : IPricingStrategy
{
    private const decimal MinimumPrice = 49.99m;
    private const decimal LoyaltyDiscount = 10m;

    public string ProviderName => "ArcticAir";

    public decimal CalculatePricePerPassenger(decimal baseFare)
    {
        var discountedPrice = Math.Round((baseFare * 1.20m) - LoyaltyDiscount, 2);

        return Math.Max(discountedPrice, MinimumPrice);
    }
}
