namespace SkyRoute.Api.Pricing
{
    public class ArcticAirPricingStrategy : IPricingStrategy
    {
        private const decimal MinimumPrice = 49.99m;

        public string ProviderName => "ArcticAir";

        public decimal CalculatePricePerPassenger(decimal baseFare) =>
            Math.Max(Math.Round((baseFare * 1.20m), 2), MinimumPrice);


    }
}