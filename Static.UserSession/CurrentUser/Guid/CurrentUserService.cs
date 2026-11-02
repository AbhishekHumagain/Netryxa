using Microsoft.AspNetCore.Http;
using Static.UserSession.CurrentUser.Service;
using System.Diagnostics.CodeAnalysis;

namespace Static.UserSession.CurrentUser.Guid;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : Service.CurrentUserService<System.Guid>(httpContextAccessor), ICurrentUserService
{
    public override System.Guid UserId => TryGetUserGuid(out var guid) ? guid.Value : System.Guid.Empty;
    public override System.Guid? UserIdOrNull => TryGetUserGuid(out var guid) ? guid : null;

    private bool TryGetUserGuid([NotNullWhen(true)] out System.Guid? userGuid)
    {
        if (TryGetClaimValue(IdClaimType, out var userId))
        {
            if (System.Guid.TryParse(userId, out var guid))
            {
                userGuid = guid;
                return true;
            }
        }

        userGuid = null;
        return false;
    }
}

public class CurrentUserService<TRoleEnum>(IHttpContextAccessor httpContextAccessor) : CurrentUserService<System.Guid, TRoleEnum>(httpContextAccessor), ICurrentUserService<TRoleEnum> where TRoleEnum : struct, Enum
{
    public override System.Guid UserId => TryGetUserGuid(out var guid) ? guid.Value : System.Guid.Empty;
    public override System.Guid? UserIdOrNull => TryGetUserGuid(out var guid) ? guid : null;

    private bool TryGetUserGuid([NotNullWhen(true)] out System.Guid? userGuid)
    {
        if (TryGetClaimValue(IdClaimType, out var userId))
        {
            if (System.Guid.TryParse(userId, out var guid))
            {
                userGuid = guid;
                return true;
            }
        }

        userGuid = null;
        return false;
    }
}