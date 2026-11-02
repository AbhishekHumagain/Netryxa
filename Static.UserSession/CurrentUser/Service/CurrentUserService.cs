using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Static.UserSession.CurrentUser.Service;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public bool IsUserAuthenticated => httpContextAccessor.HttpContext.User?.Identity?.IsAuthenticated ?? false;
    protected virtual string IdClaimType => ClaimTypes.NameIdentifier;
    protected virtual string NameClaimType => ClaimTypes.Name;
    protected virtual string EmailClaimType => ClaimTypes.Email;
    protected virtual string RoleClaimType => ClaimTypes.Role;
    public string UserId => TryGetClaimValue(IdClaimType, out var name) ? name : Constants.Unknown;
    public string UserRole => TryGetClaimValue(RoleClaimType, out var name) ? name : Constants.Unknown;
    public string UserName => TryGetClaimValue(NameClaimType, out var name) ? name : Constants.Unknown;
    public string UserEmail => TryGetClaimValue(EmailClaimType, out var email) ? email : Constants.Unknown;
    public HttpContext HttpContext => httpContextAccessor.HttpContext;
    public string? GetRequestHeader(string key) => HttpContext.Request.Headers[key];
    public string GetClaimValue(string claimType) => TryGetClaimValue(claimType, out var value) ? value : Constants.Unknown;
    public bool TryGetClaimValue(string claimType, [NotNullWhen(true)] [NotNullIfNotNull(nameof(defaultValue))] out string? claimValue, string? defaultValue = null)
    {
        if (httpContextAccessor.HttpContext?.User.Identity is ClaimsIdentity claimsIdentity)
        {
            var claim = claimsIdentity.FindFirst(claimType);
            if (claim != null)
            {
                claimValue = claim.Value;
                return true;
            }
        }

        claimValue = defaultValue;
        return false;
    }
}

public abstract class CurrentUserService<TUserId>(IHttpContextAccessor httpContextAccessor) : CurrentUserService(httpContextAccessor), ICurrentUserService<TUserId>
    where TUserId : struct
{
    public new abstract TUserId UserId { get; }

    public abstract TUserId? UserIdOrNull { get; }
}

public abstract class CurrentUserService<TUserId, TRoleEnum>(IHttpContextAccessor httpContextAccessor) : CurrentUserService<TUserId>(httpContextAccessor), ICurrentUserService<TUserId, TRoleEnum>
    where TUserId : struct
    where TRoleEnum : struct, Enum
{
    protected virtual TRoleEnum Default => (TRoleEnum)(object)-1;
    public new TRoleEnum UserRole => TryGetUserRole(out var userRole) ? userRole.Value : Default;
    public TRoleEnum? UserRoleOrNull => TryGetUserRole(out var userRole) ? userRole : null;

    private bool TryGetUserRole([NotNullWhen(true)] out TRoleEnum? userRole)
    {
        if (TryGetClaimValue(RoleClaimType, out var roleClaimValue))
        {
            if (Enum.TryParse<TRoleEnum>(roleClaimValue, true, out var role))
            {
                userRole = role;
                return true;
            }
        }

        userRole = null;
        return false;
    }

    public bool IsUserRole(TRoleEnum role) => UserRole.Equals(role);

    public bool IsUserRoleAnyOf(out TRoleEnum userRole, params TRoleEnum[] roles)
    {
        userRole = UserRole;
        return roles.Contains(userRole);
    }
}