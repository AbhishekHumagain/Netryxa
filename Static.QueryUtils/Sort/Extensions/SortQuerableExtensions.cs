using FluentValidation;
using FluentValidation.Results;
using Static.QueryUtils.Sort.Enum;
using Static.QueryUtils.Sort.Filter;
using Static.QueryUtils.Sort.Filter.SortDirection;
using System.Linq.Expressions;
using System.Reflection;

namespace Static.QueryUtils.Sort.Extensions;

public static class SortQueryableExtensions
{
    private const string OrderByMethodName = nameof(Enumerable.OrderBy);
    private const string OrderByDescendingMethodName = nameof(Enumerable.OrderByDescending);

    #region Manual Sorting

    public static IOrderedQueryable<T> SortBy<T>(
        this IQueryable<T> source,
        ISortFilter filter,
        Func<IQueryable<T>, string, IOrderedQueryable<T>> ifAscending,
        Func<IQueryable<T>, string, IOrderedQueryable<T>> ifDescending,
        Func<IQueryable<T>, IOrderedQueryable<T>>? ifNoSortField = null,
        SortOrder defaultSortOrder = SortOrder.Asc)
    {
        var field = filter.SortBy;
        if (string.IsNullOrWhiteSpace(field)) return ifNoSortField?.Invoke(source) ?? source.OrderBy(x => 0);
        var isAscending = filter.IsAscending ?? defaultSortOrder == SortOrder.Asc;
        return isAscending
            ? ifAscending.Invoke(source, field)
            : ifDescending.Invoke(source, field);
    }

    public static IOrderedQueryable<T> SortBy<T>(
        this IQueryable<T> source,
        ISortDirectionFilter filter,
        Func<IQueryable<T>, IOrderedQueryable<T>> ifAscending,
        Func<IQueryable<T>, IOrderedQueryable<T>> ifDescending,
        Func<IQueryable<T>, IOrderedQueryable<T>>? ifNoSortDirection = null)
    {
        if (filter.IsAscending == null) return ifNoSortDirection?.Invoke(source) ?? source.OrderBy(x => 0);
        return filter.IsAscending.Value
            ? ifAscending.Invoke(source)
            : ifDescending.Invoke(source);
    }

    #endregion

    #region Automatic Sorting

    public static IOrderedQueryable<T> SortBy<T>(
        this IQueryable<T> source,
        ISortFilter filter,
        Func<IQueryable<T>, IOrderedQueryable<T>>? ifNoSortField = null,
        SortOrder defaultSortOrder = SortOrder.Asc)
    {
        var sortField = filter.SortBy;
        if (sortField == null) return ifNoSortField?.Invoke(source) ?? source.OrderBy(x => 0);

        var isAscending = filter.IsAscending ?? defaultSortOrder == SortOrder.Asc;

        var type = typeof(T);
        var propertyInfo = typeof(T).GetProperty(sortField, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                           ?? throw new ValidationException([new ValidationFailure(nameof(ISortFilter.SortBy), "Given sort field does not exist.", sortField)]);

        var parameter = Expression.Parameter(type, "x");
        var memberAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
        var orderByLambda = Expression.Lambda(memberAccess, parameter);
        var methodName = isAscending ? OrderByMethodName : OrderByDescendingMethodName;
        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            [type, propertyInfo.PropertyType],
            source.Expression,
            Expression.Quote(orderByLambda));

        return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(resultExpression);
    }

    #endregion
}