namespace Static.QueryUtils.Search;

public record SearchFilterReq : ISearchFilterReq
{
    public virtual string Search { get; init; }
}