namespace Static.QueryUtils.Sort.Filter.SortBy;

public record SortByFilterReq : ISortByFilterReq
{
    public virtual string SortBy { get; init; }
}