using Microsoft.AspNetCore.Mvc;

namespace SkyRoute.Api.Validation;

public static class ValidationProblemFactory
{
    public static ValidationProblemDetails Create(IReadOnlyDictionary<string, string[]> errors)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };

        foreach (var error in errors)
        {
            problemDetails.Errors.Add(error.Key, error.Value);
        }

        return problemDetails;
    }
}
