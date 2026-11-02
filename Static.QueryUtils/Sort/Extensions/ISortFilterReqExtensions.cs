using Static.QueryUtils.Sort.Filter;

namespace Static.QueryUtils.Sort.Extensions;

public static class ISortFilterReqExtensions
{
    public static ISortFilter ToSortFilter(this ISortFilterReq sortFilterReq)
        => new SortFilter { SortBy = sortFilterReq.SortBy, IsAscending = sortFilterReq.IsAscending };
}