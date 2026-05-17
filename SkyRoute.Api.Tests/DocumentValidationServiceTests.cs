using SkyRoute.Api.Domain;
using SkyRoute.Api.Validation;

namespace SkyRoute.Api.Tests;

public sealed class DocumentValidationServiceTests
{
    private readonly DocumentValidationService _service = new();

    [Fact]
    public void Uses_passport_for_international_flights()
    {
        var offer = CreateOffer("AR", "US");

        var documentType = _service.GetDocumentType(offer);

        Assert.Equal(DocumentType.PassportNumber, documentType);
        Assert.True(_service.IsValid(documentType, "A1234567"));
        Assert.False(_service.IsValid(documentType, "12345"));
    }

    [Fact]
    public void Uses_national_id_for_domestic_flights()
    {
        var offer = CreateOffer("US", "US");

        var documentType = _service.GetDocumentType(offer);

        Assert.Equal(DocumentType.NationalId, documentType);
        Assert.True(_service.IsValid(documentType, "12345678"));
        Assert.False(_service.IsValid(documentType, "A1234567"));
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
