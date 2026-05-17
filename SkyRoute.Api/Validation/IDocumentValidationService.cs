using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Validation;

public interface IDocumentValidationService
{
    DocumentType GetDocumentType(FlightOffer offer);

    string GetLabel(DocumentType documentType);

    bool IsValid(DocumentType documentType, string? documentNumber);
}
