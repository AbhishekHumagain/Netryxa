using System.Net;
using Static.Results;

namespace Static.Application.EndUser.Auth;

/// <summary>Domain errors for authentication operations.</summary>
internal static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        new(HttpStatusCode.Unauthorized, "Invalid username or password.");

    public static readonly Error InvalidRefreshToken =
        new(HttpStatusCode.Unauthorized, "Refresh token is invalid or has expired.");

    public static readonly Error InvalidAccessToken =
        new(HttpStatusCode.Unauthorized, "Access token is invalid or has expired.");

    public static readonly Error LogoutFailed =
        new(HttpStatusCode.BadRequest, "Logout failed. The session may have already expired.");
}
