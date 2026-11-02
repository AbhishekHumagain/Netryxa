using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace Static.API.Middleware.ExceptionHandler;

/// <summary>
/// A fallback exception handler that provides a standardized error response 
/// for unhandled exceptions in the application.
/// </summary>
/// <remarks>
/// This handler will be executed as a last resort if no other exception handler processes the exception.
/// It uses the <see cref="ErrorResponseBuilder"/> to generate and write a consistent error response,
/// including request details and exception information (in non-production environments).
/// </remarks>
/// <param name="webHostEnvironment">The hosting environment used to determine whether to include detailed error information.</param>
public class ExceptionHandler(IWebHostEnvironment webHostEnvironment, ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    
    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unhandled exception occurred.");
        
        await ErrorResponseBuilder
              .CreateBuilder(HttpStatusCode.InternalServerError, "Something went wrong.")
              .AddType(nameof(Exception))
              .AttachContext(httpContext)
              .AttachException(exception)
              .IncludeHttpRequestInfo()
              .IncludeExceptionInfo()
              .EnableErrorDetailsIfNotProduction(webHostEnvironment)
              .WriteToResponseAsync(cancellationToken);
        
        return true;
    }
}