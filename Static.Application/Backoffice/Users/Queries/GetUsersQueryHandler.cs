using Static.Application.EndUser.Users;
using Static.Identity.Services;
using Static.MediatR.Query;
using Static.Results;

namespace Static.Application.Backoffice.Users.Queries;

internal sealed class GetUsersQueryHandler(IKeycloakAdminService adminService)
    : IQueryHandler<GetUsersQuery, IReadOnlyList<Dictionary<string, object>>>
{
    public async Task<Result<IReadOnlyList<Dictionary<string, object>>>> Handle(
        GetUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await adminService.GetUsersAsync(cancellationToken);
            return Result<IReadOnlyList<Dictionary<string, object>>>.From(users);
        }
        catch (HttpRequestException)
        {
            return UserErrors.IdentityProviderUnavailable;
        }
    }
}
