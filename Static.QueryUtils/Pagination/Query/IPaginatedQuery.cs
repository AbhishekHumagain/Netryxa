using Static.MediatR.Query;
using Static.QueryUtils.Pagination.Filter;

namespace Static.QueryUtils.Pagination.Query;

public interface IPaginatedQuery<TResponse> : IPaginationFilter, IQuery<PaginatedList<TResponse>>;