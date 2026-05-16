using Microsoft.AspNetCore.Mvc;
using SkyRoute.Api.Contracts;
using SkyRoute.Api.Services;
using SkyRoute.Api.Validation;

namespace SkyRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingResponse>> ConfirmAsync(
        BookingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var confirmation = await _bookingService.ConfirmAsync(request, cancellationToken);
            return Ok(BookingResponse.FromConfirmation(confirmation));
        }
        catch (BookingValidationException exception)
        {
            var problemDetails = new ValidationProblemDetails
            {
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest
            };

            foreach (var error in exception.Errors)
            {
                problemDetails.Errors.Add(error.Key, error.Value);
            }

            return BadRequest(problemDetails);
        }
    }
}
