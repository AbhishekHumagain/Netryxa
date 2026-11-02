using Static.QueryUtils.Pagination.Filter;

namespace Static.QueryUtils.Pagination.Extensions;

public static class PaginationFilterExtensions
{
    public static bool IsPagingDisabled(this IPaginationFilter filter) 
        => filter is { Page: 0, Size: 0 };
    
    public static bool IsPagingEnabled(this IPaginationFilter filter) 
        => !filter.IsPagingDisabled();
    
    public static int ItemsToSkip(this IPaginationFilter filter) 
        => (filter.Page - 1) * filter.Size;
    
    public static int ItemsPerPage(this IPaginationFilter filter) 
        => filter.Size;
    
    public static T[] ApplyPaging<T>(this IPaginationFilter filter, T[] source) 
        => source.Skip(filter.ItemsToSkip()).Take(filter.ItemsPerPage()).ToArray();
    
    public static IQueryable<T> ApplyPaging<T>(this IPaginationFilter filter, IQueryable<T> source) 
        => source.Skip(filter.ItemsToSkip()).Take(filter.ItemsPerPage());
    
    public static T[] ApplyPagingIfEnabled<T>(this IPaginationFilter filter, T[] source) 
        => filter.IsPagingDisabled() ? source : filter.ApplyPaging(source);
    
    public static IQueryable<T> ApplyPagingIfEnabled<T>(this IPaginationFilter filter, IQueryable<T> source) 
        => filter.IsPagingDisabled() ? source : filter.ApplyPaging(source);
}