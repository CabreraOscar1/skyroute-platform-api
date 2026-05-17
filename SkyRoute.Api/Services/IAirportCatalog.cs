using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Services;

public interface IAirportCatalog
{
    IReadOnlyCollection<Airport> GetAll();

    Airport? FindByCode(string? code);
}
