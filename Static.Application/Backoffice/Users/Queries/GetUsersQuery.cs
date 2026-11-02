using Static.MediatR.Query;

namespace Static.Application.Backoffice.Users.Queries;

/// <summary>Returns a raw list of all users in the Keycloak realm (admin use only).</summary>
public sealed record GetUsersQuery : IQuery<IReadOnlyList<Dictionary<string, object>>>;
