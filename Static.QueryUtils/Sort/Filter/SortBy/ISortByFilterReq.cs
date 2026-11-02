namespace Static.QueryUtils.Sort.Filter.SortBy;

public interface ISortByFilterReq : IFilterKind
{
    string SortBy { get; }
}