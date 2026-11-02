using Static.Identity.Models;

namespace Static.Identity.Services;

/// <summary>
/// OIDC operations against the Keycloak realm: login, logout, token refresh, userinfo,
/// and social login via the Google identity provider.
/// </summary>
public interface IKeycloakService
{
    /// <summary>Authenticate with username/password (resource-owner password grant).</summary>
    Task<TokenResponse> LoginAsync(string username, string password, CancellationToken ct = default);

    /// <summary>Revoke a session by invalidating the supplied refresh token.</summary>
    Task LogoutAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>Exchange a refresh token for a new access + refresh token pair.</summary>
    Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>Retrieve OpenID Connect user-info claims for a valid access token.</summary>
    Task<Dictionary<string, object>> GetUserInfoAsync(string accessToken, CancellationToken ct = default);

    /// <summary>
    /// Build the Keycloak authorization URL that redirects the user to Google login.
    /// The caller (mobile app / SPA) should open this URL in a browser or web-view.
    /// After the user authenticates, Keycloak redirects back to <paramref name="redirectUri"/>
    /// with <c>?code=...</c> appended.
    /// </summary>
    Task<string> GetGoogleAuthUrlAsync(string redirectUri, CancellationToken ct = default);

    /// <summary>
    /// Exchange the authorization <paramref name="code"/> returned by Keycloak's Google
    /// callback for a set of Keycloak tokens (access_token, refresh_token, id_token).
    /// <paramref name="redirectUri"/> must exactly match the value used in
    /// <see cref="GetGoogleAuthUrlAsync"/>.
    /// </summary>
    Task<TokenResponse> ExchangeCodeAsync(string code, string redirectUri, CancellationToken ct = default);
}
