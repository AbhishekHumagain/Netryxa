using Microsoft.AspNetCore.Mvc;
using Static.API.Controllers.Helper;
using Static.Application.EndUser.Auth.Commands;
using Static.Application.EndUser.Auth.Queries;

namespace Static.API.Controllers.EndUser.Auth;

/// <summary>
/// Authentication endpoints: login, logout, token refresh, user-info, and Google OAuth.
/// All operations are delegated to the Keycloak identity provider via MediatR commands/queries.
/// </summary>
[Route("api/auth")]
public sealed class AuthController : ApiControllerBase
{
    /// <summary>
    /// Authenticate with username and password.
    /// Returns Keycloak access + refresh tokens on success.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new LoginCommand(request.Username, request.Password), ct);

        return result.Match<IActionResult>(
            token  => Ok(token),
            error  => Problem(detail: error.Message, statusCode: (int)error.Code));
    }

    /// <summary>
    /// Revoke the current session by invalidating the refresh token.
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new LogoutCommand(request.RefreshToken), ct);

        return result.Match<IActionResult>(
            ()    => NoContent(),
            error => Problem(detail: error.Message, statusCode: (int)error.Code));
    }

    /// <summary>
    /// Exchange a refresh token for a new access + refresh token pair.
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new RefreshTokenCommand(request.RefreshToken), ct);

        return result.Match<IActionResult>(
            token => Ok(token),
            error => Problem(detail: error.Message, statusCode: (int)error.Code));
    }

    /// <summary>
    /// Retrieve OpenID Connect user-info claims using the caller's Bearer token.
    /// </summary>
    [HttpGet("userinfo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UserInfo(CancellationToken ct)
    {
        var authHeader = HttpContext.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authHeader) ||
            !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized(new { message = "Bearer token required." });

        var accessToken = authHeader["Bearer ".Length..];

        var result = await Mediator.Send(new GetUserInfoQuery(accessToken), ct);

        return result.Match<IActionResult>(
            info  => Ok(info),
            error => Problem(detail: error.Message, statusCode: (int)error.Code));
    }

    // ── Google OAuth (Authorization Code flow) ────────────────────────────────

    /// <summary>
    /// Get the Google OAuth authorization URL.
    /// Open this URL in a browser or web-view to start the Google login flow.
    /// Keycloak handles the Google exchange and redirects back to your
    /// <c>redirect_uri</c> with <c>?code=…&amp;session_state=…</c>.
    /// Pass that code to <c>POST /api/auth/google/callback</c>.
    /// </summary>
    /// <param name="redirectUri">
    /// The URI Keycloak redirects to after Google login.
    /// Must be registered in both Google Cloud Console and Keycloak client settings.
    /// Examples: <c>https://myapp.com/auth/callback</c> or <c>myapp://auth/callback</c>.
    /// </param>
    [HttpGet("google")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GoogleAuthUrl([FromQuery] string redirectUri, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetGoogleAuthUrlQuery(redirectUri), ct);

        return result.Match<IActionResult>(
            url   => Ok(new { authUrl = url }),
            error => Problem(detail: error.Message, statusCode: (int)error.Code));
    }

    /// <summary>
    /// Exchange the Google OAuth authorization code for Keycloak tokens.
    /// Call this after the user's browser is redirected back from Keycloak
    /// with <c>?code=…</c>.  Returns the same token structure as <c>POST /login</c>.
    /// </summary>
    [HttpPost("google/callback")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GoogleCallback([FromBody] GoogleCallbackRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(
            new ExchangeCodeCommand(request.Code, request.RedirectUri), ct);

        return result.Match<IActionResult>(
            token => Ok(token),
            error => Problem(detail: error.Message, statusCode: (int)error.Code));
    }
}

// ── Request models ─────────────────────────────────────────────────────────────
/// <summary>Login with username and password.</summary>
public sealed record LoginRequest(string Username, string Password);
/// <summary>Logout by invalidating the refresh token.</summary>
public sealed record LogoutRequest(string RefreshToken);
/// <summary>Refresh the access token using a refresh token.</summary>
public sealed record RefreshRequest(string RefreshToken);
/// <summary>Exchange a Google OAuth authorization code for Keycloak tokens.</summary>
public sealed record GoogleCallbackRequest(string Code, string RedirectUri);
