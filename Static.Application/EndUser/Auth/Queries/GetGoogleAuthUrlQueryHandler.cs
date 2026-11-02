using Static.Identity.Services;
using Static.MediatR.Query;
using Static.Results;

namespace Static.Application.EndUser.Auth.Queries;

internal sealed class GetGoogleAuthUrlQueryHandler(IKeycloakService keycloakService)
    : IQueryHandler<GetGoogleAuthUrlQuery, string>
{
    public async Task<Result<string>> Handle(GetGoogleAuthUrlQuery request, CancellationToken cancellationToken)
    {
        var url = await keycloakService.GetGoogleAuthUrlAsync(request.RedirectUri, cancellationToken);
        return Result<string>.From(url);
    }
}
