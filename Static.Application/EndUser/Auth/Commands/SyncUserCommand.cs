using Static.Application.Common.UserSync;
using Static.MediatR.Command;
using Static.Results;

namespace Static.Application.EndUser.Auth.Commands;

/// <summary>
/// Mirrors the signed-in user into the local store from their access token.
/// Kept separate from <see cref="LoginCommand"/> / <see cref="ExchangeCodeCommand"/> so the
/// database write gets its own short transaction, independent of the Keycloak round-trip.
/// </summary>
public sealed record SyncUserCommand(string AccessToken) : ICommand;

internal sealed class SyncUserCommandHandler(IUserSyncService userSync) : ICommandHandler<SyncUserCommand>
{
    public async Task<Result> Handle(SyncUserCommand request, CancellationToken cancellationToken)
    {
        // The sync never throws — a failure must not break sign-in.
        await userSync.SyncFromAccessTokenAsync(request.AccessToken, cancellationToken);
        return Result.Success();
    }
}
