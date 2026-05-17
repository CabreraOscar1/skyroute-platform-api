using Microsoft.AspNetCore.Mvc;
using SkyRoute.Api.Contracts;
using SkyRoute.Api.Services;

namespace SkyRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AirportsController : ControllerBase
{
    private readonly IAirportCatalog _airportCatalog;

    public AirportsController(IAirportCatalog airportCatalog)
    {
        _airportCatalog = airportCatalog;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AirportResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<AirportResponse>> GetAll()
    {
        var airports = _airportCatalog
            .GetAll()
            .Select(AirportResponse.FromAirport)
            .ToArray();

        return Ok(airports);
    }
}
