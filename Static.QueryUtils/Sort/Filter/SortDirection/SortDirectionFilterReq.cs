namespace Static.QueryUtils.Sort.Filter.SortDirection;

public record SortDirectionFilterReq : ISortDirectionFilterReq
{
    public virtual bool IsAscending { get; init; }
}