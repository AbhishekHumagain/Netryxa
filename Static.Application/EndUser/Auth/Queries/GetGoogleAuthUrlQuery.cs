using FluentValidation;
using Static.MediatR.Query;

namespace Static.Application.EndUser.Auth.Queries;

/// <summary>
/// Returns the Keycloak authorization URL that redirects the user to Google login.
/// The caller should open this URL in a browser / web-view.
/// After Google authenticates the user, Keycloak redirects to <see cref="RedirectUri"/>
/// with <c>?code=…</c> appended — pass that code to <c>ExchangeCodeCommand</c>.
/// </summary>
public sealed record GetGoogleAuthUrlQuery(string RedirectUri) : IQuery<string>;

public sealed class GetGoogleAuthUrlQueryValidator : AbstractValidator<GetGoogleAuthUrlQuery>
{
    public GetGoogleAuthUrlQueryValidator()
    {
        RuleFor(x => x.RedirectUri)
            .NotEmpty()
            .WithMessage("redirect_uri is required.");
    }
}
