using System.Net.Mail;
using SkyRoute.Api.Contracts;
using SkyRoute.Api.Domain;
using SkyRoute.Api.Validation;

namespace SkyRoute.Api.Services;

public sealed class BookingService : IBookingService
{
    private readonly IOfferStore _offerStore;
    private readonly IDocumentValidationService _documentValidationService;

    public BookingService(
        IOfferStore offerStore,
        IDocumentValidationService documentValidationService)
    {
        _offerStore = offerStore;
        _documentValidationService = documentValidationService;
    }

    public Task<BookingConfirmation> ConfirmAsync(
        BookingRequest request,
        CancellationToken cancellationToken)
    {
        var offer = ValidateAndGetOffer(request);
        var bookingReference = CreateBookingReference(offer);

        var confirmation = new BookingConfirmation(
            bookingReference,
            "Confirmed",
            offer.OfferId,
            offer.Provider,
            offer.FlightNumber,
            offer.TotalPrice,
            offer.Currency);

        return Task.FromResult(confirmation);
    }

    private FlightOffer ValidateAndGetOffer(BookingRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        var offer = _offerStore.FindById(request.OfferId);

        if (string.IsNullOrWhiteSpace(request.OfferId))
        {
            errors[nameof(request.OfferId)] = ["Offer id is required."];
        }
        else if (offer is null)
        {
            errors[nameof(request.OfferId)] = ["Offer id does not exist or has expired."];
        }

        if (request.PrimaryPassenger is null)
        {
            errors[nameof(request.PrimaryPassenger)] = ["Primary passenger is required."];
        }
        else
        {
            ValidatePassenger(request.PrimaryPassenger, offer, errors);
        }

        if (errors.Count > 0 || offer is null)
        {
            throw new BookingValidationException(errors);
        }

        return offer;
    }

    private void ValidatePassenger(
        PassengerRequest passenger,
        FlightOffer? offer,
        IDictionary<string, string[]> errors)
    {
        if (string.IsNullOrWhiteSpace(passenger.FullName))
        {
            errors["PrimaryPassenger.FullName"] = ["Full name is required."];
        }

        if (string.IsNullOrWhiteSpace(passenger.Email))
        {
            errors["PrimaryPassenger.Email"] = ["Email is required."];
        }
        else if (!IsValidEmail(passenger.Email))
        {
            errors["PrimaryPassenger.Email"] = ["Email format is invalid."];
        }

        if (string.IsNullOrWhiteSpace(passenger.DocumentNumber))
        {
            errors["PrimaryPassenger.DocumentNumber"] = ["Document number is required."];
            return;
        }

        if (offer is null)
        {
            return;
        }

        var documentType = _documentValidationService.GetDocumentType(offer);

        if (!_documentValidationService.IsValid(documentType, passenger.DocumentNumber))
        {
            var label = _documentValidationService.GetLabel(documentType);
            errors["PrimaryPassenger.DocumentNumber"] =
            [
                $"{label} must be {(documentType == DocumentType.NationalId ? "numeric" : "alphanumeric")} and 6 to 12 characters long."
            ];
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var address = new MailAddress(email.Trim());
            return string.Equals(address.Address, email.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static string CreateBookingReference(FlightOffer offer)
    {
        var departureDate = offer.DepartureAt.ToString("yyyyMMdd");
        var suffix = Guid.NewGuid().ToString("N")[..5].ToUpperInvariant();

        return $"SKY-{departureDate}-{suffix}";
    }
}
