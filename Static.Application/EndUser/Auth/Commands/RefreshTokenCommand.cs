using FluentValidation;
using Static.Identity.Models;
using Static.MediatR.Command;

namespace Static.Application.EndUser.Auth.Commands;

/// <summary>Exchanges a refresh token for a new access + refresh token pair.</summary>
public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<TokenResponse>;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
