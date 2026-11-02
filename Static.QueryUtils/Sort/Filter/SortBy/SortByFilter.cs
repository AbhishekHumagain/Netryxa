namespace Static.QueryUtils.Sort.Filter.SortBy;

public record SortByFilter : ISortByFilter
{
    public virtual string? SortBy { get; init; }
}