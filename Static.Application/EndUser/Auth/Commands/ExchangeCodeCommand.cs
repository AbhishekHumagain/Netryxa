using FluentValidation;
using Static.Identity.Models;
using Static.MediatR.Command;

namespace Static.Application.EndUser.Auth.Commands;

/// <summary>
/// Exchanges the authorization <c>code</c> (returned by Keycloak's Google OAuth callback)
/// for a set of Keycloak tokens: <c>access_token</c>, <c>refresh_token</c>, <c>id_token</c>.
/// <c>RedirectUri</c> must exactly match the value passed to <c>GetGoogleAuthUrlQuery</c>.
/// </summary>
public sealed record ExchangeCodeCommand(string Code, string RedirectUri) : ICommand<TokenResponse>, INonTransactionalCommand;

public sealed class ExchangeCodeCommandValidator : AbstractValidator<ExchangeCodeCommand>
{
    public ExchangeCodeCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Authorization code is required.");

        RuleFor(x => x.RedirectUri)
            .NotEmpty()
            .WithMessage("redirect_uri is required and must match the original request.");
    }
}
