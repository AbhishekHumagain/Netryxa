namespace Static.Application.Common.UserSync;

/// <summary>
/// Keeps the local <see cref="Static.Domain.Entities.User"/> store in sync with the
/// identity provider (Keycloak). Called on authentication so that a user is mirrored
/// into the database regardless of how they were created — API registration, the
/// Keycloak admin console, or a social (Google) sign-up.
/// </summary>
public interface IUserSyncService
{
    /// <summary>
    /// Upserts a local user row from the claims embedded in a Keycloak access token
    /// (JWT). Never throws — a sync failure must not break the sign-in flow.
    /// </summary>
    Task SyncFromAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);
}
