namespace Static.QueryUtils.Sort.Filter.SortDirection;

public interface ISortDirectionFilterReq : IFilterKind
{
    public bool IsAscending { get; }
}