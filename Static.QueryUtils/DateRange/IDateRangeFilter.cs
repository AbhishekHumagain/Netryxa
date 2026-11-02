namespace Static.QueryUtils.DateRange;

public interface IDateRangeFilter : IFilterKind
{
    DateTime? StartDate { get; }
    DateTime? EndDate { get; }
}