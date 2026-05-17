namespace SkyRoute.Api.Pricing;

public sealed class GlobalAirPricingStrategy : IPricingStrategy
{
    public string ProviderName => "GlobalAir";

    public decimal CalculatePricePerPassenger(decimal baseFare) =>
        Math.Round(baseFare * 1.15m, 2);
}
