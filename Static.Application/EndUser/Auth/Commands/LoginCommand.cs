using FluentValidation;
using Static.Identity.Models;
using Static.MediatR.Command;

namespace Static.Application.EndUser.Auth.Commands;

/// <summary>
/// Authenticates a user with username and password via Keycloak.
/// Runs outside the unit-of-work transaction so a slow Keycloak response can't abort it;
/// the local user sync happens in its own <see cref="SyncUserCommand"/>.
/// </summary>
public sealed record LoginCommand(string Username, string Password) : ICommand<TokenResponse>, INonTransactionalCommand;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
