using FluentValidation;
using Static.MediatR.Command;

namespace Static.Application.Backoffice.Users.Commands;

/// <summary>Permanently deletes a user from Keycloak by their user ID (admin only).</summary>
public sealed record DeleteUserCommand(string UserId) : ICommand;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
