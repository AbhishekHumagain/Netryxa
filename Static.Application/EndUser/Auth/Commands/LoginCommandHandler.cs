using MediatR;
using Static.Identity.Models;
using Static.Identity.Services;
using Static.MediatR.Command;
using Static.Results;

namespace Static.Application.EndUser.Auth.Commands;

internal sealed class LoginCommandHandler(
    IKeycloakService keycloakService,
    ISender sender)
    : ICommandHandler<LoginCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await keycloakService.LoginAsync(request.Username, request.Password, cancellationToken);

            // Mirror the authenticated user into the local store (self-healing sync,
            // independent of how the account was originally created).
            await sender.Send(new SyncUserCommand(token.AccessToken), cancellationToken);

            return Result<TokenResponse>.From(token);
        }
        catch (HttpRequestException)
        {
            return AuthErrors.InvalidCredentials;
        }
    }
}
