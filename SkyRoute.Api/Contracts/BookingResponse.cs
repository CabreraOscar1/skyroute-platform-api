using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Contracts;

public sealed record BookingResponse(
    string BookingReference,
    string Status,
    string OfferId,
    string Provider,
    string FlightNumber,
    decimal TotalPrice,
    string Currency)
{
    public static BookingResponse FromConfirmation(BookingConfirmation confirmation) =>
        new(
            confirmation.BookingReference,
            confirmation.Status,
            confirmation.OfferId,
            confirmation.Provider,
            confirmation.FlightNumber,
            confirmation.TotalPrice,
            confirmation.Currency);
}
