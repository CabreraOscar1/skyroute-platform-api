namespace SkyRoute.Api.Pricing;

public interface IPricingStrategy
{
    string ProviderName { get; }

    decimal CalculatePricePerPassenger(decimal baseFare);
}
