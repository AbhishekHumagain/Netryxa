using FluentValidation;
using Static.MediatR.Command;

namespace Static.Application.EndUser.Auth.Commands;

/// <summary>Revokes a Keycloak session by invalidating the refresh token.</summary>
public sealed record LogoutCommand(string RefreshToken) : ICommand;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
