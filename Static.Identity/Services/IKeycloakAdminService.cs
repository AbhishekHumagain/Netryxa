namespace Static.Identity.Services;

/// <summary>
/// Admin-level operations against the Keycloak Admin REST API:
/// user creation, role assignment, and user management.
/// </summary>
public interface IKeycloakAdminService
{
    /// <summary>
    /// Creates a new user in the configured realm and assigns the given realm role.
    /// Returns the Keycloak user ID (UUID).
    /// </summary>
    Task<string> CreateUserAsync(
        string username,
        string email,
        string firstName,
        string lastName,
        string password,
        string roleName,
        CancellationToken ct = default);

    /// <summary>Returns a raw list of all users in the realm.</summary>
    Task<IReadOnlyList<Dictionary<string, object>>> GetUsersAsync(CancellationToken ct = default);

    /// <summary>Permanently deletes a user by their Keycloak user ID.</summary>
    Task DeleteUserAsync(string userId, CancellationToken ct = default);
}
