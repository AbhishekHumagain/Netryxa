using Static.QueryUtils.Pagination.Extensions;
using Microsoft.EntityFrameworkCore;
using Static.QueryUtils.Pagination.Filter;

namespace Static.QueryUtils.Pagination;

public class PaginatedList<T>
{
    public IEnumerable<T> Items { get; }
    public int TotalItems { get; }
    public int Page { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    private PaginatedList(IEnumerable<T> items, IPaginationFilter filter, int totalItems)
    {
        if (totalItems == 0 || filter.IsPagingDisabled())
        {
            Page = 1;
            TotalPages = 1;
        }
        else
        {
            Page = filter.Page;
            TotalPages = (int)Math.Ceiling(totalItems / (double)filter.Size);
        }

        TotalItems = totalItems;
        Items = items;
    }

    public static PaginatedList<T> Create(IEnumerable<T> items, IPaginationFilter filter, int totalItems)
        => new(items, filter, totalItems);

    public static PaginatedList<T> Create(IEnumerable<T> items, int page, int size, int totalItems)
        => new(items, new PaginationFilter { Page = page, Size = size }, totalItems);

    public static PaginatedList<T> Create(IEnumerable<T> source, IPaginationFilter filter, int? totalItems = null)
    {
        var array = source.ToArray();
        totalItems ??= array.Length;
        var items = filter.ApplyPagingIfEnabled(array);
        return new(items, filter, totalItems.Value);
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, IPaginationFilter filter, int? totalItems = null, CancellationToken cancellationToken = default)
    {
        totalItems ??= await source.CountAsync(cancellationToken);
        var items = await filter.ApplyPagingIfEnabled(source).ToArrayAsync(cancellationToken);
        return new(items, filter, totalItems.Value);
    }

    public static PaginatedList<T> Empty() => Create(Array.Empty<T>(), 0, 0, 0);
}