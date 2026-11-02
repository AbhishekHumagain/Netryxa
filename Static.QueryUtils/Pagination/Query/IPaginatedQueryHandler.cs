using Static.MediatR.Query;

namespace Static.QueryUtils.Pagination.Query;

public interface IPaginatedQueryHandler<in TQuery, TResponse> : IQueryHandler<TQuery, PaginatedList<TResponse>>
    where TQuery : IPaginatedQuery<TResponse>;