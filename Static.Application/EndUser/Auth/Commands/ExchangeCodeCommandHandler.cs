using MediatR;
using Static.Identity.Models;
using Static.Identity.Services;
using Static.MediatR.Command;
using Static.Results;

namespace Static.Application.EndUser.Auth.Commands;

internal sealed class ExchangeCodeCommandHandler(
    IKeycloakService keycloakService,
    ISender sender)
    : ICommandHandler<ExchangeCodeCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(ExchangeCodeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await keycloakService.ExchangeCodeAsync(
                request.Code, request.RedirectUri, cancellationToken);

            // Social (Google) sign-ups never hit the registration API, so mirror the
            // user into the local store here on first sign-in.
            await sender.Send(new SyncUserCommand(token.AccessToken), cancellationToken);

            return Result<TokenResponse>.From(token);
        }
        catch (HttpRequestException)
        {
            // Keycloak rejected the code (expired, already used, redirect_uri mismatch, etc.)
            return AuthErrors.InvalidCredentials;
        }
    }
}
