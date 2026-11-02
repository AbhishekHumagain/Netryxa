namespace Static.QueryUtils.Search;

public interface ISearchFilter : IFilterKind
{
    string? Search { get; }
}