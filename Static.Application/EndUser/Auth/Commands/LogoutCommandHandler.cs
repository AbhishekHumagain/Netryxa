using Static.Identity.Services;
using Static.MediatR.Command;
using Static.Results;

namespace Static.Application.EndUser.Auth.Commands;

internal sealed class LogoutCommandHandler(IKeycloakService keycloakService)
    : ICommandHandler<LogoutCommand>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await keycloakService.LogoutAsync(request.RefreshToken, cancellationToken);
            return Result.Success();
        }
        catch (HttpRequestException)
        {
            return AuthErrors.LogoutFailed;
        }
    }
}
