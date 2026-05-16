namespace SkyRoute.Api.Domain;

public sealed record Passenger(
    string FullName,
    string Email,
    string DocumentNumber);
