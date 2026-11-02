using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Static.Identity.Options;

namespace Static.Identity.Services;

/// <summary>
/// Communicates with the Keycloak Admin REST API using a short-lived master-realm admin token.
/// </summary>
internal sealed class KeycloakAdminService(
    HttpClient httpClient,
    IOptions<KeycloakOptions> options,
    ILogger<KeycloakAdminService> logger) : IKeycloakAdminService
{
    private readonly KeycloakOptions _opt = options.Value;

    // ── Endpoints ─────────────────────────────────────────────────────────────

    private string AdminTokenEndpoint =>
        $"{_opt.BaseUrl}/realms/master/protocol/openid-connect/token";

    private string UsersEndpoint =>
        $"{_opt.BaseUrl}/admin/realms/{_opt.Realm}/users";

    private string RolesEndpoint =>
        $"{_opt.BaseUrl}/admin/realms/{_opt.Realm}/roles";

    // ── Public API ────────────────────────────────────────────────────────────

    public async Task<string> CreateUserAsync(
        string username, string email, string firstName, string lastName,
        string password, string roleName, CancellationToken ct = default)
    {
        var token = await GetAdminTokenAsync(ct);

        var userId = await CreateKeycloakUserAsync(token, username, email, firstName, lastName, ct);
        await SetPasswordAsync(token, userId, password, ct);

        var role = await GetRealmRoleAsync(token, roleName, ct);
        await AssignRoleAsync(token, userId, role, ct);

        logger.LogInformation(
            "Created Keycloak user '{Username}' with role '{Role}' (id={UserId})",
            username, roleName, userId);

        return userId;
    }

    public async Task<IReadOnlyList<Dictionary<string, object>>> GetUsersAsync(CancellationToken ct = default)
    {
        var token = await GetAdminTokenAsync(ct);
        using var req = BuildGet(UsersEndpoint, token);
        var res = await httpClient.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json) ?? [];
    }

    public async Task DeleteUserAsync(string userId, CancellationToken ct = default)
    {
        var token = await GetAdminTokenAsync(ct);
        using var req = new HttpRequestMessage(HttpMethod.Delete, $"{UsersEndpoint}/{userId}");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var res = await httpClient.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private async Task<string> GetAdminTokenAsync(CancellationToken ct)
    {
        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"]  = "admin-cli",
            ["username"]   = _opt.AdminUsername,
            ["password"]   = _opt.AdminPassword
        };

        var res = await httpClient.PostAsync(AdminTokenEndpoint, new FormUrlEncodedContent(form), ct);
        res.EnsureSuccessStatusCode();

        var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync(ct));
        return doc.RootElement.GetProperty("access_token").GetString()
               ?? throw new InvalidOperationException("Could not obtain admin token from Keycloak.");
    }

    private async Task<string> CreateKeycloakUserAsync(
        string token, string username, string email,
        string firstName, string lastName, CancellationToken ct)
    {
        var payload = new
        {
            username,
            email,
            firstName,
            lastName,
            enabled = true
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, UsersEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var res = await httpClient.SendAsync(request, ct);

        if (res.StatusCode == HttpStatusCode.Conflict)
            throw new InvalidOperationException($"User '{username}' already exists in Keycloak.");

        res.EnsureSuccessStatusCode();

        // Keycloak returns the new user id in the Location header
        var location = res.Headers.Location?.ToString()
                       ?? throw new InvalidOperationException(
                           "Keycloak did not return a Location header after user creation.");

        return location.Split('/').Last();
    }

    private async Task SetPasswordAsync(string token, string userId, string password, CancellationToken ct)
    {
        var payload = new { type = "password", value = password, temporary = false };
        var url = $"{UsersEndpoint}/{userId}/reset-password";

        using var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var res = await httpClient.SendAsync(request, ct);
        res.EnsureSuccessStatusCode();
    }

    private async Task<JsonElement> GetRealmRoleAsync(string token, string roleName, CancellationToken ct)
    {
        using var req = BuildGet($"{RolesEndpoint}/{roleName}", token);
        var res = await httpClient.SendAsync(req, ct);

        if (res.StatusCode == HttpStatusCode.NotFound)
            throw new InvalidOperationException(
                $"Realm role '{roleName}' does not exist. Create it in the Keycloak Admin UI first.");

        res.EnsureSuccessStatusCode();
        return JsonDocument.Parse(await res.Content.ReadAsStringAsync(ct)).RootElement;
    }

    private async Task AssignRoleAsync(string token, string userId, JsonElement role, CancellationToken ct)
    {
        var url = $"{UsersEndpoint}/{userId}/role-mappings/realm";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(
            JsonSerializer.Serialize(new[] { role }), Encoding.UTF8, "application/json");

        var res = await httpClient.SendAsync(request, ct);
        res.EnsureSuccessStatusCode();
    }

    private static HttpRequestMessage BuildGet(string url, string token)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return req;
    }
}
