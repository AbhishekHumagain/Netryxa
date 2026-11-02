using Static.Results;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Static.API.Middleware.ExceptionHandler;

/// <summary>
/// A builder class to construct an <see cref="Error"/> object with detailed error information.
/// This follows the Builder pattern, supporting fluent configuration of error metadata, trace info,
/// exception diagnostics, and HTTP request context.
/// </summary>
public class ErrorResponseBuilder
{
    /// <summary>
    /// Gets the HTTP status code associated with the error.
    /// </summary>
    public HttpStatusCode StatusCode { get; private set; }

    /// <summary>
    /// Gets the primary error message.
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Gets the current HTTP context, if attached.
    /// </summary>
    public HttpContext? HttpContext { get; private set; }

    /// <summary>
    /// Gets a value indicating whether HTTP request details are included in the error.
    /// </summary>
    public bool IsHttpRequestDetailsIncluded { get; private set; }

    /// <summary>
    /// Gets the captured exception, if attached.
    /// </summary>
    public Exception? Exception { get; private set; }

    /// <summary>
    /// Gets a value indicating whether exception details are included in the error.
    /// </summary>
    public bool IsExceptionDetailsIncluded { get; private set; }

    /// <summary>
    /// Gets the additional metadata associated with the error.
    /// </summary>
    public Dictionary<string, object?> AdditionalData { get; } = new();

    /// <summary>
    /// Gets a value indicating whether detailed error information is enabled.
    /// </summary>
    public bool AreErrorDetailsEnabled { get; private set; }

    /// <summary>
    /// Initializes a new <see cref="ErrorResponseBuilder"/> with required properties.
    /// </summary>
    public static ErrorResponseBuilder CreateBuilder(HttpStatusCode statusCode, string message) =>
        new() { StatusCode = statusCode, Message = message, };

    /// <summary>
    /// Attaches the current <see cref="HttpContext"/> to the error.
    /// </summary>
    [MemberNotNull(nameof(HttpContext))]
    public ErrorResponseBuilder AttachContext(HttpContext httpContext)
    {
        HttpContext = httpContext;
        return this;
    }

    /// <summary>
    /// Attaches the captured <see cref="Exception"/> to the error.
    /// </summary>
    [MemberNotNull(nameof(Exception))]
    public ErrorResponseBuilder AttachException(Exception exception)
    {
        Exception = exception;
        return this;
    }

    /// <summary>
    /// Includes HTTP request information in the error details.
    /// </summary>
    [MemberNotNull(nameof(HttpContext))]
    public ErrorResponseBuilder IncludeHttpRequestInfo()
    {
        ArgumentNullException.ThrowIfNull(HttpContext);
        IsHttpRequestDetailsIncluded = true;
        return this;
    }

    /// <summary>
    /// Includes exception information in the error details.
    /// </summary>
    [MemberNotNull(nameof(Exception))]
    public ErrorResponseBuilder IncludeExceptionInfo()
    {
        ArgumentNullException.ThrowIfNull(Exception);
        IsExceptionDetailsIncluded = true;
        return this;
    }

    /// <summary>
    /// Adds a value to the 'details' section of the error response.
    /// </summary>
    public ErrorResponseBuilder AddDetails(object? value, bool onlyIfErrorDetailsEnabled = false) =>
        AddExtension("details", value, onlyIfErrorDetailsEnabled);

    /// <summary>
    /// Adds a type identifier to the error response.
    /// </summary>
    public ErrorResponseBuilder AddType(string value)
    {
        AdditionalData["type"] = value;
        return this;
    }

    /// <summary>
    /// Adds a key-value pair to the error response extensions.
    /// </summary>
    public ErrorResponseBuilder AddExtension(string key, object? value, bool onlyIfErrorDetailsEnabled = false)
    {
        if (onlyIfErrorDetailsEnabled && !AreErrorDetailsEnabled) return this;
        AdditionalData[key] = value;
        return this;
    }

    /// <summary>
    /// Enables or disables detailed error information depending on the environment.
    /// </summary>
    public ErrorResponseBuilder EnableErrorDetailsIfNotProduction(IWebHostEnvironment env)
        => Apply(env.IsDevelopment());

    /// <summary>
    /// Enables or disables detailed error information manually.
    /// </summary>
    public ErrorResponseBuilder EnableErrorDetails(bool enabled)
        => Apply(enabled);

    private ErrorResponseBuilder Apply(bool enabled)
    {
        AreErrorDetailsEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Builds the <see cref="Error"/> object, enriched with optional exception and request metadata.
    /// </summary>
    public Error BuildError()
    {
        if (AreErrorDetailsEnabled)
        {
            if (IsExceptionDetailsIncluded)
            {
                ArgumentNullException.ThrowIfNull(Exception);
                AdditionalData["exception"] = CreateExceptionData(Exception);
            }

            if (IsHttpRequestDetailsIncluded)
            {
                ArgumentNullException.ThrowIfNull(HttpContext);
                AdditionalData["httpRequest"] = CreateHttpRequestData(HttpContext);
            }
        }

        return new(StatusCode, Message) { TraceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? HttpContext?.TraceIdentifier, Extensions = AdditionalData };
    }

    /// <summary>
    /// Writes the error object to the HTTP response as JSON.
    /// </summary>
    public Task WriteToResponseAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(HttpContext);
        var error = BuildError();
        var httpResponse = HttpContext.Response;
        httpResponse.StatusCode = (int)StatusCode;
        return httpResponse.WriteAsJsonAsync(error, cancellationToken: cancellationToken);
    }

    #region Helpers

    /// <summary>
    /// Represents detailed metadata for an <see cref="Exception"/>.
    /// </summary>
    private record ExceptionData(
        string Type,
        string Message,
        int HResult,
        string? StackTrace,
        string? Source,
        ExceptionData? InnerException);

    /// <summary>
    /// Recursively builds exception metadata up to a depth of 5.
    /// </summary>
    private static ExceptionData CreateExceptionData(Exception ex, int maxDepth = 5, int depth = 0)
    {
        ExceptionData? inner = null;
        if (ex.InnerException != null && depth < maxDepth)
        {
            inner = CreateExceptionData(ex.InnerException, maxDepth, depth + 1);
        }

        return new(
            Type: ex.GetType().FullName ?? ex.GetType().Name,
            Message: ex.Message,
            HResult: ex.HResult,
            StackTrace: ex.StackTrace,
            Source: ex.Source,
            InnerException: inner
        );
    }

    /// <summary>
    /// A claim within a user principal.
    /// </summary>
    private record RequestData(string RequestId, string Method, string Path, string? Endpoint);

    // Deliberately omit headers, cookies, query strings, claims, and request payloads.
    private static RequestData CreateHttpRequestData(HttpContext context) => new(
        context.TraceIdentifier, context.Request.Method, context.Request.Path,
        context.GetEndpoint()?.DisplayName);

    #endregion
}