namespace Static.QueryUtils.Sort.Filter.SortDirection;

public interface ISortDirectionFilter : IFilterKind
{
    public bool? IsAscending { get; }
}