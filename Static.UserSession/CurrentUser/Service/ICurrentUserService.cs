using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace Static.UserSession.CurrentUser.Service;

public interface ICurrentUserService
{
    bool IsUserAuthenticated { get; }
    string UserId { get; }
    string UserRole { get; }
    string UserName { get; }
    string UserEmail { get; }
    string GetClaimValue(string claimType);
    HttpContext HttpContext { get; }
    string? GetRequestHeader(string key);
    bool TryGetClaimValue(string claimType, [NotNullWhen(true)] [NotNullIfNotNull(nameof(defaultValue))] out string? claimValue, string? defaultValue = null);
}


/// <summary>
/// Defines the contract for a service to provide information about the current user.
/// </summary>
/// <typeparam name="TUserId">
/// The type of the user identifier. Must be a value type that implements <see cref="IEquatable{T}"/>.
/// </typeparam>
public interface ICurrentUserService<TUserId>: ICurrentUserService where TUserId : struct
{
    new TUserId UserId { get; }
    TUserId? UserIdOrNull { get; }
}

public interface ICurrentUserService<TUserId, TRoleEnum> : ICurrentUserService<TUserId>
    where TUserId : struct
    where TRoleEnum : struct, Enum
{
    new TRoleEnum UserRole { get; }
    TRoleEnum? UserRoleOrNull { get; }
    bool IsUserRole(TRoleEnum role);
    bool IsUserRoleAnyOf(out TRoleEnum userRole, params TRoleEnum[] roles);
}