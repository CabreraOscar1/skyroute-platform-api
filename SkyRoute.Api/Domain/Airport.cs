namespace SkyRoute.Api.Domain;

public sealed record Airport(
    string Code,
    string Name,
    string City,
    string CountryCode);
