using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Static.API.Controllers.Helper;
using Static.API.Filter;
using Static.Application.Backoffice.Users.Commands;
using Static.Application.Backoffice.Users.Queries;
using Static.Helper.Constants.EnumCollections;
using Static.Identity.Extensions;
using ServiceCollectionExtensions = Static.Identity.Extensions.ServiceCollectionExtensions;

namespace Static.API.Controllers.Backoffice.Users;

/// <summary>
/// Back-office user management: list and delete Keycloak users.
/// Restricted to SuperAdmin only.
/// </summary>
[Authorize(AuthenticationSchemes = ServiceCollectionExtensions.KeycloakScheme)]
[Route("api-bo/users")]
public sealed class BoUsersController : ApiBoControllerBase
{
    /// <summary>
    /// List all users in the Keycloak realm.
    /// Accessible by: SuperAdmin.
    /// </summary>
    [HttpGet]
    [PermissionRole("read:users", StaticRole.SuperAdmin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetUsersQuery(), ct);

        return result.Match<IActionResult>(
            users => Ok(new { data = users, total = users.Count }),
            error => Problem(detail: error.Message, statusCode: (int)error.Code));
    }

    /// <summary>
    /// Permanently delete a user from Keycloak.
    /// Accessible by: SuperAdmin.
    /// </summary>
    [HttpDelete("{userId}")]
    [PermissionRole("delete:user", StaticRole.SuperAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> DeleteUser(string userId, CancellationToken ct)
    {
        var result = await Mediator.Send(new DeleteUserCommand(userId), ct);

        return result.Match<IActionResult>(
            NoContent,
            error => Problem(detail: error.Message, statusCode: (int)error.Code));
    }
}
