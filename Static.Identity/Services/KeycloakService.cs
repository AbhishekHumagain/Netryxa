using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Static.Identity.Models;
using Static.Identity.Options;

namespace Static.Identity.Services;

/// <summary>
/// Communicates with Keycloak's OpenID Connect endpoints for token operations
/// and social login via the Google identity provider.
/// </summary>
internal sealed class KeycloakService(
    HttpClient httpClient,
    IOptions<KeycloakOptions> options) : IKeycloakService
{
    private readonly KeycloakOptions _opt = options.Value;

    private string TokenEndpoint =>
        $"{_opt.BaseUrl}/realms/{_opt.Realm}/protocol/openid-connect/token";

    private string LogoutEndpoint =>
        $"{_opt.BaseUrl}/realms/{_opt.Realm}/protocol/openid-connect/logout";

    private string UserInfoEndpoint =>
        $"{_opt.BaseUrl}/realms/{_opt.Realm}/protocol/openid-connect/userinfo";

    private string AuthEndpoint =>
        $"{_opt.BaseUrl}/realms/{_opt.Realm}/protocol/openid-connect/auth";

    // ── Standard OIDC ────────────────────────────────────────────────────────

    public async Task<TokenResponse> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var form = new Dictionary<string, string>
        {
            ["grant_type"]    = "password",
            ["client_id"]     = _opt.ClientId,
            ["client_secret"] = _opt.ClientSecret,
            ["username"]      = username,
            ["password"]      = password,
            ["scope"]         = "openid profile email"
        };

        var response = await httpClient.PostAsync(TokenEndpoint, new FormUrlEncodedContent(form), ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TokenResponse>(json)!;
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        var form = new Dictionary<string, string>
        {
            ["client_id"]     = _opt.ClientId,
            ["client_secret"] = _opt.ClientSecret,
            ["refresh_token"] = refreshToken
        };

        var response = await httpClient.PostAsync(LogoutEndpoint, new FormUrlEncodedContent(form), ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var form = new Dictionary<string, string>
        {
            ["grant_type"]    = "refresh_token",
            ["client_id"]     = _opt.ClientId,
            ["client_secret"] = _opt.ClientSecret,
            ["refresh_token"] = refreshToken
        };

        var response = await httpClient.PostAsync(TokenEndpoint, new FormUrlEncodedContent(form), ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TokenResponse>(json)!;
    }

    public async Task<Dictionary<string, object>> GetUserInfoAsync(string accessToken, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, UserInfoEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
    }

    // ── Google / Social login (Authorization Code flow) ──────────────────────

    public Task<string> GetGoogleAuthUrlAsync(string redirectUri, CancellationToken ct = default)
    {
        // Build the Keycloak authorization URL.
        // kc_idp_hint=google instructs Keycloak to skip its own login page
        // and forward the user directly to Google.
        var url = $"{AuthEndpoint}" +
                  $"?client_id={Uri.EscapeDataString(_opt.ClientId)}" +
                  $"&response_type=code" +
                  $"&scope=openid%20profile%20email" +
                  $"&kc_idp_hint=google" +
                  $"&redirect_uri={Uri.EscapeDataString(redirectUri)}";

        return Task.FromResult(url);
    }

    public async Task<TokenResponse> ExchangeCodeAsync(string code, string redirectUri, CancellationToken ct = default)
    {
        // Standard OIDC authorization_code grant.
        // Keycloak has already handled the Google OAuth exchange internally;
        // we just trade the authorization code for Keycloak-issued tokens.
        var form = new Dictionary<string, string>
        {
            ["grant_type"]    = "authorization_code",
            ["code"]          = code,
            ["redirect_uri"]  = redirectUri,
            ["client_id"]     = _opt.ClientId,
            ["client_secret"] = _opt.ClientSecret
        };

        var response = await httpClient.PostAsync(TokenEndpoint, new FormUrlEncodedContent(form), ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TokenResponse>(json)!;
    }
}
