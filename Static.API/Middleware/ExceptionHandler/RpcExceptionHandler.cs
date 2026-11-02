using Static.API.Grpc;
using Google.Rpc;
using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;

namespace Static.API.Middleware.ExceptionHandler;

/// <summary>
/// Exception handler for <see cref="RpcException"/>.
/// Converts gRPC errors into structured HTTP responses with appropriate status codes and details.
/// </summary>
/// <param name="webHostEnvironment">Provides information about the web hosting environment.</param>
public class RpcExceptionHandler(IWebHostEnvironment webHostEnvironment, ILogger<RpcExceptionHandler> logger)
    : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not RpcException rpcException) return false;

        logger.LogError(exception, "gRPC request failed — StatusCode: {StatusCode}, Detail: {Detail}",
            rpcException.StatusCode,
            rpcException.Status.Detail
        );

        var builder = ErrorResponseBuilder
                      .CreateBuilder(rpcException.StatusCode.ToHttpStatusCode(), rpcException.Status.Detail)
                      .AttachContext(httpContext)
                      .AttachException(exception)
                      .AddType(nameof(RpcException))
                      .IncludeHttpRequestInfo()
                      .IncludeExceptionInfo()
                      .EnableErrorDetailsIfNotProduction(webHostEnvironment);

        var rpcStatus = rpcException.GetRpcStatus();
        if (rpcStatus != null && rpcStatus.Details.Count != 0)
        {
            var details = new List<object>();
            foreach (var msg in rpcStatus.UnpackDetailMessages())
            {
                var type = msg.Descriptor.FullName;
                object? detail = msg switch
                {
                    BadRequest badRequest =>
                        badRequest.FieldViolations
                                  .GroupBy(x => x.Field)
                                  .ToDictionary(x => x.Key, x => x.ToArray()),


                    ErrorInfo errorInfo => new
                    {
                        errorInfo.Domain,
                        errorInfo.Reason,
                        Metadata = errorInfo.Metadata
                                            .ToDictionary(x => x.Key, x => x.Value),
                    },

                    DebugInfo debugInfo when builder.AreErrorDetailsEnabled => new
                    {
                        debugInfo.Detail, StackEntries = debugInfo.StackEntries.Select(x => x).ToArray()
                    },

                    _ => null

                    // others
                    // Refer to https://github.com/googleapis/googleapis/blob/master/google/rpc/error_details.proto
                    // Can use custom proto (Recommended)
                };
                if (detail is not null) details.Add(new { type, Details = detail });
            }

            if (details.Count != 0) builder.AddDetails(details);
        }

        await builder.WriteToResponseAsync(cancellationToken);

        return true;
    }
}