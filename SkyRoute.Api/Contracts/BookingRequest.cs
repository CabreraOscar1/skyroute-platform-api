namespace SkyRoute.Api.Contracts;

public sealed record BookingRequest(
    string? OfferId,
    PassengerRequest? PrimaryPassenger);
