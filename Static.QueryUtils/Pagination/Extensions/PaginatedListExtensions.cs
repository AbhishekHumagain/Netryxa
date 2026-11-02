using Microsoft.EntityFrameworkCore;
using Static.QueryUtils.Pagination.Filter;

namespace Static.QueryUtils.Pagination.Extensions;

public static class PaginatedListExtensions
{
    public static Task<PaginatedList<TDestination>> ToPaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, IPaginationFilter paginationFilter, CancellationToken cancellationToken = default) 
        where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), paginationFilter, cancellationToken: cancellationToken);
    
    public static Task<PaginatedList<TDestination>> ToPaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, IPaginationFilter paginationFilter, int totalItems,  CancellationToken cancellationToken = default) 
        where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), paginationFilter, totalItems, cancellationToken);

    public static PaginatedList<TDestination> ToPaginatedList<TDestination>(this IEnumerable<TDestination> enumerable, IPaginationFilter paginationFilter) 
        where TDestination : class
        => PaginatedList<TDestination>.Create(enumerable, paginationFilter);
    
    public static PaginatedList<TDestination> ToPaginatedList<TDestination>(this IEnumerable<TDestination> enumerable, IPaginationFilter paginationFilter, int totalItems) 
        where TDestination : class
        => PaginatedList<TDestination>.Create(enumerable, paginationFilter, totalItems);
}