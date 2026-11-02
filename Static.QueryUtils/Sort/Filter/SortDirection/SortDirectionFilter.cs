namespace Static.QueryUtils.Sort.Filter.SortDirection;

public record SortDirectionFilter : ISortDirectionFilter
{
    public virtual bool? IsAscending { get; init; }
}