using System.Collections.Concurrent;
using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Services;

public sealed class InMemoryOfferStore : IOfferStore
{
    private static readonly TimeSpan OfferTtl = TimeSpan.FromMinutes(30);
    private readonly ConcurrentDictionary<string, StoredOffer> _offers = new();

    public void SaveMany(IEnumerable<FlightOffer> offers)
    {
        PruneExpiredOffers();

        foreach (var offer in offers)
        {
            _offers[offer.OfferId] = new StoredOffer(
                offer,
                DateTimeOffset.UtcNow.Add(OfferTtl));
        }
    }

    public FlightOffer? FindById(string? offerId)
    {
        if (string.IsNullOrWhiteSpace(offerId))
        {
            return null;
        }

        if (!_offers.TryGetValue(offerId.Trim(), out var storedOffer))
        {
            return null;
        }

        if (storedOffer.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            _offers.TryRemove(offerId.Trim(), out _);
            return null;
        }

        return storedOffer.Offer;
    }

    private void PruneExpiredOffers()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var offer in _offers)
        {
            if (offer.Value.ExpiresAt <= now)
            {
                _offers.TryRemove(offer.Key, out _);
            }
        }
    }

    private sealed record StoredOffer(
        FlightOffer Offer,
        DateTimeOffset ExpiresAt);
}
