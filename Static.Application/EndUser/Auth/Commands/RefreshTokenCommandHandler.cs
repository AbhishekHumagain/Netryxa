using Static.Identity.Models;
using Static.Identity.Services;
using Static.MediatR.Command;
using Static.Results;

namespace Static.Application.EndUser.Auth.Commands;

internal sealed class RefreshTokenCommandHandler(IKeycloakService keycloakService)
    : ICommandHandler<RefreshTokenCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await keycloakService.RefreshAsync(request.RefreshToken, cancellationToken);
            return Result<TokenResponse>.From(token);
        }
        catch (HttpRequestException)
        {
            return AuthErrors.InvalidRefreshToken;
        }
    }
}
