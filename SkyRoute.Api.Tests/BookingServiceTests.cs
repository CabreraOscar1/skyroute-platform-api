using SkyRoute.Api.Contracts;
using SkyRoute.Api.Domain;
using SkyRoute.Api.Services;
using SkyRoute.Api.Validation;

namespace SkyRoute.Api.Tests;

public sealed class BookingServiceTests
{
    [Fact]
    public async Task Confirm_returns_booking_reference_for_valid_offer()
    {
        var offerStore = new InMemoryOfferStore();
        var offer = CreateOffer("AR", "US");
        offerStore.SaveMany([offer]);
        var service = new BookingService(offerStore, new DocumentValidationService());
        var request = new BookingRequest(
            offer.OfferId,
            new PassengerRequest("Ana Perez", "ana.perez@example.com", "A1234567"));

        var confirmation = await service.ConfirmAsync(request, CancellationToken.None);

        Assert.Equal("Confirmed", confirmation.Status);
        Assert.Equal(offer.OfferId, confirmation.OfferId);
        Assert.StartsWith("SKY-20260610-", confirmation.BookingReference);
    }

    [Fact]
    public async Task Confirm_rejects_unknown_offer()
    {
        var service = new BookingService(new InMemoryOfferStore(), new DocumentValidationService());
        var request = new BookingRequest(
            "OFF-GA-MISSING-01",
            new PassengerRequest("Ana Perez", "ana.perez@example.com", "A1234567"));

        await Assert.ThrowsAsync<BookingValidationException>(() =>
            service.ConfirmAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task Confirm_validates_domestic_document_as_national_id()
    {
        var offerStore = new InMemoryOfferStore();
        var offer = CreateOffer("US", "US");
        offerStore.SaveMany([offer]);
        var service = new BookingService(offerStore, new DocumentValidationService());
        var request = new BookingRequest(
            offer.OfferId,
            new PassengerRequest("John Smith", "john.smith@example.com", "A1234567"));

        await Assert.ThrowsAsync<BookingValidationException>(() =>
            service.ConfirmAsync(request, CancellationToken.None));
    }

    private static FlightOffer CreateOffer(string originCountry, string destinationCountry) =>
        new(
            "OFF-GA-ABC12345-01",
            "GlobalAir",
            "GA123",
            new Airport("AAA", "Origin Airport", "Origin", originCountry),
            new Airport("BBB", "Destination Airport", "Destination", destinationCountry),
            new DateTime(2026, 6, 10, 8, 0, 0),
            new DateTime(2026, 6, 10, 10, 0, 0),
            120,
            CabinClass.Economy,
            100m,
            115m,
            1,
            115m,
            "USD");
}
