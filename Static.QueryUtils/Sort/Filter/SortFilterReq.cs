namespace Static.QueryUtils.Sort.Filter;

public record SortFilterReq : ISortFilterReq
{
    public virtual bool IsAscending { get; init;  }
    public virtual string SortBy { get; init; }
}