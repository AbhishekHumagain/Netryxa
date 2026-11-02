using System.ComponentModel.DataAnnotations;

namespace Static.QueryUtils.DateRange;

public record DateRangeFilter : IDateRangeFilter
{
    [DataType(DataType.Date)] public virtual DateTime? StartDate { get; init; }
    [DataType(DataType.Date)] public virtual DateTime? EndDate { get; init; }
}