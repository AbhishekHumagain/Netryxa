using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Static.Identity.Options;
using Static.Identity.Services;

namespace Static.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Named JWT Bearer scheme for Keycloak-issued tokens.
    /// Use on controllers: [Authorize(AuthenticationSchemes = KeycloakScheme)]
    /// </summary>
    public const string KeycloakScheme = "Keycloak";

    /// <summary>
    /// Registers the Keycloak identity services:
    ///  - <see cref="IKeycloakService"/>  for OIDC token operations (login / logout / refresh / userinfo)
    ///  - <see cref="IKeycloakAdminService"/> for user management (create / delete / list users)
    ///  - A named JWT Bearer scheme ("Keycloak") that validates Keycloak-issued RS256 tokens and
    ///    maps realm/client roles to <see cref="ClaimTypes.Role"/> for [Authorize(Roles="...")] support.
    ///  - Sets "Keycloak" as the default authenticate, challenge, and forbid scheme so that
    ///    plain [Authorize] attributes work without specifying the scheme explicitly.
    /// </summary>
    /// <param name="isProduction">
    ///   When <c>true</c>, enforces HTTPS metadata on the JWT Bearer handler (security requirement).
    ///   Set to <c>false</c> only for local development where Keycloak runs without TLS.
    /// </param>
    public static IServiceCollection AddKeycloakIdentity(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isProduction = false)
    {
        // Bind options from appsettings "Keycloak" section
        services.Configure<KeycloakOptions>(configuration.GetSection(KeycloakOptions.Section));

        // Register typed HTTP clients — DI injects HttpClient + IOptions<KeycloakOptions>
        services.AddHttpClient<IKeycloakService, KeycloakService>();
        services.AddHttpClient<IKeycloakAdminService, KeycloakAdminService>();

        // JWT Bearer scheme for Keycloak token validation
        var section  = configuration.GetSection(KeycloakOptions.Section);
        var baseUrl  = section["BaseUrl"];
        var realm    = section["Realm"];
        var clientId = section["ClientId"];

        if (!string.IsNullOrWhiteSpace(baseUrl) && !string.IsNullOrWhiteSpace(realm))
        {
            var authority = section["Authority"] ?? $"{baseUrl}/realms/{realm}";
            var metadataAddress = section["MetadataAddress"];

            // Set "Keycloak" as the application-wide default scheme.
            // This means [Authorize] without an explicit scheme uses Keycloak JWT validation.
            // The legacy symmetric-key Bearer scheme (AddAuthService) remains registered
            // for backward compatibility but is no longer the default.
            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = KeycloakScheme;
                        options.DefaultChallengeScheme    = KeycloakScheme;
                        options.DefaultForbidScheme       = KeycloakScheme;
                    })
                    .AddJwtBearer(KeycloakScheme, options =>
                    {
                        options.Authority = authority;
                        if (!string.IsNullOrWhiteSpace(metadataAddress)) options.MetadataAddress = metadataAddress;

                        // Security: require HTTPS in production.
                        // In development Keycloak typically runs over HTTP.
                        options.RequireHttpsMetadata = isProduction;

                        options.TokenValidationParameters = new()
                        {
                            ValidateIssuer           = true,
                            ValidIssuer              = authority,
                            // Keycloak tokens carry "aud": "account" by default.
                            // We validate it explicitly to prevent token reuse across services.
                            ValidateAudience         = true,
                            ValidAudience            = section["Audience"] ?? clientId,
                            ValidateLifetime         = true,
                            // Zero clock skew — tokens are short-lived; do not allow drift.
                            ClockSkew                = TimeSpan.Zero,
                            ValidateIssuerSigningKey = true,
                            // Map Keycloak claims to ASP.NET Core conventions
                            RoleClaimType            = ClaimTypes.Role,
                            NameClaimType            = "preferred_username"
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnTokenValidated = context =>
                            {
                                MapKeycloakRoles(context, clientId);
                                return Task.CompletedTask;
                            },
                            // Security: log authentication failures for observability
                            OnAuthenticationFailed = context =>
                            {
                                var logger = context.HttpContext.RequestServices
                                    .GetRequiredService<Microsoft.Extensions.Logging.ILogger<JwtBearerHandler>>();
                                logger.LogWarning("Keycloak JWT authentication failed: {Error}",
                                    context.Exception.Message);
                                return Task.CompletedTask;
                            }
                        };
                    });
        }

        return services;
    }

    /// <summary>
    /// Activates Authentication and Authorization middleware.
    /// Call inside <c>app.UseRouting()</c> … <c>app.MapControllers()</c>.
    /// </summary>
    public static IApplicationBuilder UseKeycloakIdentity(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    /// <summary>
    /// Extracts Keycloak realm and client roles from the raw JWT payload
    /// and injects them as <see cref="ClaimTypes.Role"/> claims so standard
    /// [Authorize(Roles = "Student")] attributes work.
    /// </summary>
    private static void MapKeycloakRoles(TokenValidatedContext context, string? clientId)
    {
        if (context.Principal?.Identity is not ClaimsIdentity identity) return;

        JsonElement root;
        try
        {
            var parts = context.SecurityToken.UnsafeToString().Split('.');
            if (parts.Length < 2) return;
            var json = PadBase64(parts[1]);
            root = JsonDocument.Parse(Convert.FromBase64String(json)).RootElement.Clone();
        }
        catch { return; }

        // 1. Realm roles: { "realm_access": { "roles": ["Student", ...] } }
        if (root.TryGetProperty("realm_access", out var realmAccess) &&
            realmAccess.TryGetProperty("roles", out var realmRoles))
        {
            foreach (var role in realmRoles.EnumerateArray())
            {
                var name = role.GetString();
                if (!string.IsNullOrWhiteSpace(name))
                    identity.AddClaim(new Claim(ClaimTypes.Role, name));
            }
        }

        // 2. Client roles: { "resource_access": { "<clientId>": { "roles": [...] } } }
        if (!string.IsNullOrWhiteSpace(clientId) &&
            root.TryGetProperty("resource_access", out var resourceAccess) &&
            resourceAccess.TryGetProperty(clientId, out var clientAccess) &&
            clientAccess.TryGetProperty("roles", out var clientRoles))
        {
            foreach (var role in clientRoles.EnumerateArray())
            {
                var name = role.GetString();
                if (!string.IsNullOrWhiteSpace(name))
                    identity.AddClaim(new Claim(ClaimTypes.Role, name));
            }
        }
    }

    private static string PadBase64(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        return (base64.Length % 4) switch
        {
            2 => base64 + "==",
            3 => base64 + "=",
            _ => base64
        };
    }
}
