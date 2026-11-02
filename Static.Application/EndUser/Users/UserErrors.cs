using System.Net;
using Static.Results;

namespace Static.Application.EndUser.Users;

/// <summary>Domain errors for user management operations.</summary>
internal static class UserErrors
{
    public static Error AlreadyExists(string username) =>
        new(HttpStatusCode.Conflict, $"User '{username}' already exists.");

    public static readonly Error IdentityProviderUnavailable =
        new(HttpStatusCode.BadGateway, "The identity provider is unavailable. Please try again later.");
}
