namespace Static.QueryUtils.Sort.Filter.SortBy;

public interface ISortByFilter : IFilterKind
{
    string? SortBy { get; }
}