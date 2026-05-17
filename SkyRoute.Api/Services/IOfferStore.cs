using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Services;

public interface IOfferStore
{
    void SaveMany(IEnumerable<FlightOffer> offers);

    FlightOffer? FindById(string? offerId);
}
