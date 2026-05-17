namespace SkyRoute.Api.Domain;

public sealed record BookingConfirmation(
    string BookingReference,
    string Status,
    string OfferId,
    string Provider,
    string FlightNumber,
    decimal TotalPrice,
    string Currency);
