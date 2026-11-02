namespace Static.Identity.Options;

public sealed class KeycloakOptions
{
    public const string Section = "Keycloak";

    /// <summary>Keycloak base URL, e.g. http://localhost:8080</summary>
    public string BaseUrl { get; init; } = string.Empty;

    /// <summary>Target realm for end-users, e.g. "static"</summary>
    public string Realm { get; init; } = string.Empty;

    /// <summary>OIDC client ID registered in the realm</summary>
    public string ClientId { get; init; } = string.Empty;

    /// <summary>OIDC client secret</summary>
    public string ClientSecret { get; init; } = string.Empty;

    /// <summary>Keycloak admin username (master realm)</summary>
    public string AdminUsername { get; init; } = "admin";

    /// <summary>Keycloak admin password (master realm)</summary>
    public string AdminPassword { get; init; } = "admin";
}
