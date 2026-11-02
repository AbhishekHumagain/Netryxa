using Static.Helper.Constants.EnumCollections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Static.API.Filter;

/// <summary>
/// Authorization filter that validates the caller is authenticated via Keycloak
/// and carries one of the required <see cref="StaticRole"/> realm roles.
///
/// Usage:
///   [PermissionRole("read:students", StaticRole.Teacher, StaticRole.SuperAdmin)]
///
/// Keycloak realm roles must match the StaticRole enum names exactly
/// (e.g. create roles named "Student", "Teacher", "SuperAdmin" in Keycloak Admin UI).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class PermissionRoleAttribute(string permission, params StaticRole[] roles)
    : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    public StaticRole[] Roles { get; set; } = roles;
    public string Permission { get; set; } = permission;

    public PermissionRoleAttribute() : this(string.Empty, []) { }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
            return;

        await CheckRoleAsync(context);
    }

    private Task CheckRoleAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // 1. Must be authenticated
        if (user.Identity is not { IsAuthenticated: true })
        {
            context.Result = new UnauthorizedResult();
            return Task.CompletedTask;
        }

        // 2. No roles specified → only authentication is required
        if (Roles.Length == 0)
            return Task.CompletedTask;

        // 3. Collect all role claims (Keycloak sends multiple ClaimTypes.Role claims)
        var userRoles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (userRoles.Count == 0)
        {
            context.Result = new ForbidResult();
            return Task.CompletedTask;
        }

        // 4. SuperAdmin bypasses all role checks
        if (userRoles.Contains(StaticRole.SuperAdmin.ToString()))
            return Task.CompletedTask;

        // 5. Check whether any of the caller's roles matches the required roles
        var hasRole = Roles.Any(required =>
            userRoles.Contains(required.ToString()));

        if (!hasRole)
        {
            context.Result = new ForbidResult();
            return Task.CompletedTask;
        }

        // 6. Passed — let the action execute
        return Task.CompletedTask;
    }
}
