namespace SkyRoute.Api.Pricing;

public sealed class BudgetWingsPricingStrategy : IPricingStrategy
{
    private const decimal MinimumPrice = 29.99m;

    public string ProviderName => "BudgetWings";

    public decimal CalculatePricePerPassenger(decimal baseFare) =>
        Math.Max(Math.Round(baseFare * 0.90m, 2), MinimumPrice);
}
