namespace SkyRoute.Api.Validation;

public sealed class FlightSearchValidationException : Exception
{
    public FlightSearchValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("The flight search request is invalid.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
