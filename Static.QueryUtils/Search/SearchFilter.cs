namespace Static.QueryUtils.Search;

public record SearchFilter : ISearchFilter
{
    public virtual string? Search { get; init; }
}