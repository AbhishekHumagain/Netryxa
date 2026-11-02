using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace Static.API.Middleware.ExceptionHandler;

/// <summary>
/// Exception handler for <see cref="ValidationException"/> thrown by FluentValidation
/// and returns a structured HTTP 422 Unprocessable Entity response.
/// </summary>
public class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger) : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException) return false;

        logger.LogWarning(validationException, "Validation failed: {Message}", validationException.Message);

        await ErrorResponseBuilder
              .CreateBuilder(HttpStatusCode.UnprocessableContent, validationException.Message)
              .AttachContext(httpContext)
              .AddType(nameof(ValidationException))
              .AddDetails(
                  validationException.Errors
                                     .GroupBy(x => x.PropertyName)
                                     .Select(x =>
                                         new
                                         {
                                             PropertyName = x.Key,
                                             ErrorMessages = x.Select(y => y.ErrorMessage).ToArray()
                                         }))
              .WriteToResponseAsync(cancellationToken);
        return true;
    }
}