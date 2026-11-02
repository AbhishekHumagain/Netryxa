using FluentValidation;
using Static.MediatR.Query;

namespace Static.Application.EndUser.Auth.Queries;

/// <summary>Retrieves OpenID Connect user-info claims for the supplied access token.</summary>
public sealed record GetUserInfoQuery(string AccessToken) : IQuery<Dictionary<string, object>>;

public sealed class GetUserInfoQueryValidator : AbstractValidator<GetUserInfoQuery>
{
    public GetUserInfoQueryValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
    }
}
