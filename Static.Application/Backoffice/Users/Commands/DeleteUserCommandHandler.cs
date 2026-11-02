using Static.Application.EndUser.Users;
using Static.Identity.Services;
using Static.MediatR.Command;
using Static.Results;

namespace Static.Application.Backoffice.Users.Commands;

internal sealed class DeleteUserCommandHandler(IKeycloakAdminService adminService)
    : ICommandHandler<DeleteUserCommand>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await adminService.DeleteUserAsync(request.UserId, cancellationToken);
            return Result.Success();
        }
        catch (HttpRequestException)
        {
            return UserErrors.IdentityProviderUnavailable;
        }
    }
}
