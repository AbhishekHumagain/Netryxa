using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Static.Domain.Entities;
using Static.EntityFrameworkCore.Repository.Guid;
using Static.Helper.Constants.EnumCollections;

namespace Static.Application.Common.UserSync;

/// <summary>
/// Decodes the (already Keycloak-issued and API-validated) access token to read the
/// user's identity claims and mirror them into the local <see cref="User"/> table.
/// The DB row Id equals the token 'sub' — the same value used as OwnerId /
/// CreatorUserId on content — so audit relationships line up.
/// </summary>
internal sealed class UserSyncService(
    IGenericRepository<User> userRepo,
    ILogger<UserSyncService> logger) : IUserSyncService
{
    // Priority order used when a token carries more than one recognised app role.
    private static readonly StaticRole[] RolePriority =
    [
        StaticRole.SuperAdmin, StaticRole.Teacher, StaticRole.Student
    ];

    public async Task SyncFromAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!TryReadClaims(accessToken, out var claims))
                return;

            if (!Guid.TryParse(claims.Sub, out var userId))
                return;

            var existing = await userRepo.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);

            if (existing is null)
            {
                await userRepo.InsertAsync(new User
                {
                    Id           = userId,
                    FullName     = claims.FullName,
                    EmailAddress = claims.Email,
                    Role         = claims.Role
                }, cancellationToken);
                return;
            }

            // Keep the local copy fresh if the IdP details changed.
            var changed = false;
            if (!string.IsNullOrWhiteSpace(claims.FullName) && existing.FullName != claims.FullName)
            {
                existing.FullName = claims.FullName;
                changed = true;
            }
            if (!string.IsNullOrWhiteSpace(claims.Email) && existing.EmailAddress != claims.Email)
            {
                existing.EmailAddress = claims.Email;
                changed = true;
            }
            if (existing.Role != claims.Role)
            {
                existing.Role = claims.Role;
                changed = true;
            }

            if (changed)
                userRepo.Update(existing);
        }
        catch (Exception ex)
        {
            // Never let user-sync break authentication.
            logger.LogWarning(ex, "[UserSync] Failed to mirror Keycloak user into local store.");
        }
    }

    private static bool TryReadClaims(string accessToken, out (string Sub, string FullName, string Email, StaticRole Role) claims)
    {
        claims = default;

        var parts = accessToken.Split('.');
        if (parts.Length < 2)
            return false;

        using var doc = JsonDocument.Parse(DecodeBase64Url(parts[1]));
        var root = doc.RootElement;

        var sub = GetString(root, "sub");
        if (string.IsNullOrWhiteSpace(sub))
            return false;

        var email = GetString(root, "email");
        var username = GetString(root, "preferred_username");

        var fullName = GetString(root, "name");
        if (string.IsNullOrWhiteSpace(fullName))
        {
            var given = GetString(root, "given_name");
            var family = GetString(root, "family_name");
            fullName = $"{given} {family}".Trim();
        }
        if (string.IsNullOrWhiteSpace(fullName))
            fullName = username ?? sub;

        claims = (
            sub,
            fullName,
            email ?? username ?? $"{sub}@unknown.local",
            ResolveRole(root));
        return true;
    }

    private static StaticRole ResolveRole(JsonElement root)
    {
        var roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (root.TryGetProperty("realm_access", out var realmAccess) &&
            realmAccess.TryGetProperty("roles", out var realmRoles) &&
            realmRoles.ValueKind == JsonValueKind.Array)
        {
            foreach (var r in realmRoles.EnumerateArray())
            {
                var name = r.GetString();
                if (!string.IsNullOrWhiteSpace(name))
                    roles.Add(name);
            }
        }

        foreach (var role in RolePriority)
            if (roles.Contains(role.ToString()))
                return role;

        // Unknown/absent app role → treat as a Student by default.
        return StaticRole.Student;
    }

    private static string? GetString(JsonElement root, string name) =>
        root.TryGetProperty(name, out var el) && el.ValueKind == JsonValueKind.String
            ? el.GetString()
            : null;

    private static byte[] DecodeBase64Url(string input)
    {
        var s = input.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }
        return Convert.FromBase64String(s);
    }
}

public static class UserSyncServiceExtensions
{
    public static IServiceCollection AddUserSyncService(this IServiceCollection services) =>
        services.AddScoped<IUserSyncService, UserSyncService>();
}
