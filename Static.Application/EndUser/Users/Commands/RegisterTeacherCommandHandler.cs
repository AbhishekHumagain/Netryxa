using Static.Domain.Entities;
using Static.EntityFrameworkCore.Repository.Guid;
using Static.Helper.Constants.EnumCollections;
using Static.Identity.Services;
using Static.MediatR.Command;
using Static.Results;

namespace Static.Application.EndUser.Users.Commands;

internal sealed class RegisterTeacherCommandHandler(
    IKeycloakAdminService adminService,
    IGenericRepository<User> userRepo)
    : ICommandHandler<RegisterTeacherCommand, string>
{
    private const string TeacherRole = "Teacher";

    public async Task<Result<string>> Handle(RegisterTeacherCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await adminService.CreateUserAsync(
                request.Username,
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password,
                TeacherRole,
                cancellationToken);

            // Mirror the Keycloak user into the local store so it can be referenced
            // by content audit fields and surfaced in admin dashboards. The local Id
            // matches the Keycloak user id (the token 'sub' used as OwnerId/CreatorUserId).
            await userRepo.InsertAsync(new User
            {
                Id           = Guid.Parse(userId),
                FullName     = $"{request.FirstName} {request.LastName}".Trim(),
                EmailAddress = request.Email,
                Role         = StaticRole.Teacher
            }, cancellationToken);

            return Result<string>.From(userId);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return UserErrors.AlreadyExists(request.Username);
        }
        catch (HttpRequestException)
        {
            return UserErrors.IdentityProviderUnavailable;
        }
    }
}
