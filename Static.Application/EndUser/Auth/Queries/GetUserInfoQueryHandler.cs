using Static.Identity.Services;
using Static.MediatR.Query;
using Static.Results;

namespace Static.Application.EndUser.Auth.Queries;

internal sealed class GetUserInfoQueryHandler(IKeycloakService keycloakService)
    : IQueryHandler<GetUserInfoQuery, Dictionary<string, object>>
{
    public async Task<Result<Dictionary<string, object>>> Handle(
        GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var info = await keycloakService.GetUserInfoAsync(request.AccessToken, cancellationToken);
            return Result<Dictionary<string, object>>.From(info);
        }
        catch (HttpRequestException)
        {
            return AuthErrors.InvalidAccessToken;
        }
    }
}
