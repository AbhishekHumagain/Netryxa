namespace Static.QueryUtils.Pagination.Filter;

public interface IPaginationFilter : IFilterKind
{
    int Page { get; }
    int Size { get; }
}