using System.Linq.Expressions;

namespace Static.EntityFrameworkCore.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>>? ifTrue = null,
        Expression<Func<T, bool>>? ifFalse = null)
        => condition
            ? ifTrue == null
                ? query
                : query.Where(ifTrue)
            : ifFalse == null
                ? query
                : query.Where(ifFalse);

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool? condition,
        Expression<Func<T, bool>>? ifNull = null,
        Expression<Func<T, bool>>? ifTrue = null,
        Expression<Func<T, bool>>? ifFalse = null)
        => condition == null
            ? ifNull == null
                ? query
                : query.Where(ifNull)
            : condition.Value
                ? ifTrue == null
                    ? query
                    : query.Where(ifTrue)
                : ifFalse == null
                    ? query
                    : query.Where(ifFalse);
}