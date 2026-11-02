namespace Static.QueryUtils.Search;

public interface ISearchFilterReq : IFilterKind
{
    string Search { get; }
}