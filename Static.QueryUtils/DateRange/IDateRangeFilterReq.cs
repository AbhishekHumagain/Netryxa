namespace Static.QueryUtils.DateRange;

public interface IDateRangeFilterReq : IFilterKind
{
    DateTime StartDate { get; }
    DateTime EndDate { get; }
}