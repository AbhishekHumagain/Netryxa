using Static.QueryUtils.Pagination.Filter;

namespace Static.QueryUtils.Pagination.Query;

public record PaginatedQuery<TResponse> : PaginationFilter, IPaginatedQuery<TResponse>;