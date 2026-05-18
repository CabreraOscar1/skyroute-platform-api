using System.Text.RegularExpressions;
using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Validation;

public sealed class DocumentValidationService : IDocumentValidationService
{
    public DocumentType GetDocumentType(FlightOffer offer) =>
        offer.Origin.CountryCode == offer.Destination.CountryCode
            ? DocumentType.NationalId
            : DocumentType.PassportNumber;

    public string GetLabel(DocumentType documentType) =>
        documentType switch
        {
            DocumentType.NationalId => "National ID",
            DocumentType.PassportNumber => "Passport Number",
            _ => "Document Number"
        };

    public bool IsValid(DocumentType documentType, string? documentNumber)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            return false;
        }

        var trimmedDocument = documentNumber.Trim();

        return documentType switch
        {
            DocumentType.NationalId => Regex.IsMatch(trimmedDocument, @"^\d{6,12}$"),
            DocumentType.PassportNumber => Regex.IsMatch(trimmedDocument, @"^[a-zA-Z0-9]{6,12}$"),
            _ => false
        };
    }
}
