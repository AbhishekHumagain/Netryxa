namespace Static.QueryUtils.Pagination.Filter;

public record PaginationFilter : IPaginationFilter
{
    public virtual int Page { get; init; }
    public virtual int Size { get; init; }
    
    public static PaginationFilter From(int page, int size) => new() { Page = page, Size = size };
}