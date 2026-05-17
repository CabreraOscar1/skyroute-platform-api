namespace SkyRoute.Api.Validation;

public sealed class BookingValidationException : Exception
{
    public BookingValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("The booking request is invalid.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
