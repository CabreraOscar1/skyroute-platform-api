using Microsoft.AspNetCore.Mvc;
using SkyRoute.Api.Contracts;
using SkyRoute.Api.Services;
using SkyRoute.Api.Validation;

namespace SkyRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class FlightsController : ControllerBase
{
    private readonly IFlightSearchService _flightSearchService;

    public FlightsController(IFlightSearchService flightSearchService)
    {
        _flightSearchService = flightSearchService;
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(FlightSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FlightSearchResponse>> SearchAsync(
        FlightSearchRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _flightSearchService.SearchAsync(request, cancellationToken);
            return Ok(FlightSearchResponse.FromResult(result));
        }
        catch (FlightSearchValidationException exception)
        {
            return BadRequest(ValidationProblemFactory.Create(exception.Errors));
        }
    }
}
