namespace Static.QueryUtils.Sort.Filter;

public record SortFilter : ISortFilter
{
    public virtual bool? IsAscending { get; init; }
    public virtual string? SortBy { get; init; }
}