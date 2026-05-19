namespace SkyRoute.Api.Services
{
    public class DiscountService
    {
        private readonly decimal _discountPercentage;

        public DiscountService(decimal discountPercentage)
        {
            _discountPercentage = discountPercentage;
        }

        public decimal ApplyDiscount(decimal price)
        {
            var discountMultiplier = 1 - _discountPercentage;
            return Math.Round(price * discountMultiplier, 2);
        }
    }
}
