using SkyRoute.Api.Contracts;
using SkyRoute.Api.Domain;

namespace SkyRoute.Api.Services;

public interface IBookingService
{
    Task<BookingConfirmation> ConfirmAsync(
        BookingRequest request,
        CancellationToken cancellationToken);
}
