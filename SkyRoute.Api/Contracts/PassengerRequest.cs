namespace SkyRoute.Api.Contracts;

public sealed record PassengerRequest(
    string? FullName,
    string? Email,
    string? DocumentNumber);
