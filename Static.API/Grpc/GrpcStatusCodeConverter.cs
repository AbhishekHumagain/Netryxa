using Grpc.Core;
using System.Net;

namespace Static.API.Grpc;

/// <summary>
/// Provides conversion between gRPC status codes and HTTP status codes following common API translation patterns
/// </summary>
public static class GrpcStatusCodeConverter
{
    
    /// <summary>
    /// Converts a gRPC status code to its corresponding HTTP status code
    /// </summary>
    /// <param name="grpcStatusCode">The gRPC status code to convert</param>
    /// <returns>
    /// The most appropriate HTTP status code representation for the gRPC status.
    /// Returns <see cref="HttpStatusCode.InternalServerError"/> for unmapped codes.
    /// </returns>
    /// <remarks>
    /// <para>Mapping follows Google's API mapping recommendations with additional common patterns:</para>
    /// <list type="bullet">
    ///   <item>Uses non-standard 499 status for client-closed requests (gRPC CANCELLED)</item>
    ///   <item>Treats resource exhaustion (gRPC RESOURCE_EXHAUSTED) as HTTP 429 Too Many Requests</item>
    ///   <item>Groups common client errors under 400 Bad Request</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// var grpcCode = StatusCode.PermissionDenied;
    /// HttpStatusCode httpStatus = grpcCode.ToHttpStatusCode();
    /// // Returns HttpStatusCode.Forbidden (403)
    /// </code>
    /// </example>
    public static HttpStatusCode ToHttpStatusCode(this StatusCode grpcStatusCode) =>
        grpcStatusCode switch
        {
            StatusCode.OK => HttpStatusCode.OK,
            StatusCode.Cancelled => (HttpStatusCode)499, // Client Closed Request (non-standard)
            StatusCode.InvalidArgument => HttpStatusCode.BadRequest,
            StatusCode.DeadlineExceeded => HttpStatusCode.GatewayTimeout,
            StatusCode.NotFound => HttpStatusCode.NotFound,
            StatusCode.AlreadyExists => HttpStatusCode.Conflict,
            StatusCode.PermissionDenied => HttpStatusCode.Forbidden,
            StatusCode.ResourceExhausted => HttpStatusCode.TooManyRequests,
            StatusCode.FailedPrecondition => HttpStatusCode.BadRequest,
            StatusCode.Aborted => HttpStatusCode.Conflict,
            StatusCode.OutOfRange => HttpStatusCode.BadRequest,
            StatusCode.Unimplemented => HttpStatusCode.NotImplemented,
            StatusCode.Internal => HttpStatusCode.InternalServerError,
            StatusCode.Unavailable => HttpStatusCode.ServiceUnavailable,
            StatusCode.DataLoss => HttpStatusCode.InternalServerError,
            StatusCode.Unauthenticated => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };
}