using FluentValidation;
using FluentValidation.Results;
using Static.QueryUtils.Sort.Enum;
using Static.QueryUtils.Sort.Filter;
using Static.QueryUtils.Sort.Filter.SortDirection;
using System.Linq.Expressions;
using System.Reflection;

namespace Static.QueryUtils.Sort.Extensions;

public static class SortEnumerableExtensions
{
    #region Manual Sorting

    public static IOrderedEnumerable<T> SortBy<T>(
        this IEnumerable<T> source,
        ISortFilter filter,
        Func<IEnumerable<T>, string, IOrderedEnumerable<T>> ifAscending,
        Func<IEnumerable<T>, string, IOrderedEnumerable<T>> ifDescending,
        Func<IEnumerable<T>, IOrderedEnumerable<T>>? ifNoSortField = null,
        SortOrder defaultSortOrder = SortOrder.Asc)
    {
        var field = filter.SortBy;
        if (string.IsNullOrWhiteSpace(field)) return ifNoSortField?.Invoke(source) ?? source.OrderBy(x => 0);
        var isAscending = filter.IsAscending ?? defaultSortOrder == SortOrder.Asc;
        return isAscending
            ? ifAscending.Invoke(source, field)
            : ifDescending.Invoke(source, field);
    }

    public static IOrderedEnumerable<T> SortBy<T>(
        this IEnumerable<T> source,
        ISortDirectionFilter filter,
        Func<IEnumerable<T>, IOrderedEnumerable<T>> ifAscending,
        Func<IEnumerable<T>, IOrderedEnumerable<T>> ifDescending,
        Func<IEnumerable<T>, IOrderedEnumerable<T>>? ifNoSortDirection = null)
    {
        if (filter.IsAscending == null) return ifNoSortDirection?.Invoke(source) ?? source.OrderBy(x => 0);
        return filter.IsAscending.Value
            ? ifAscending.Invoke(source)
            : ifDescending.Invoke(source);
    }

    #endregion

    #region Automatic Sorting

    public static IOrderedEnumerable<T> SortBy<T>(
        this IEnumerable<T> source,
        ISortFilter filter,
        Func<IEnumerable<T>, IOrderedEnumerable<T>>? ifNoSortField = null,
        SortOrder defaultSortOrder = SortOrder.Asc)
    {
        var sortField = filter.SortBy;
        if (sortField == null) return ifNoSortField?.Invoke(source) ?? source.OrderBy(x => 0);

        var isAscending = filter.IsAscending ?? defaultSortOrder == SortOrder.Asc;

        var propertyInfo = typeof(T).GetProperty(sortField, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                           ?? throw new ValidationException([new ValidationFailure(nameof(ISortFilter.SortBy), "Given sort field does not exist.", sortField)]);
        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.Property(parameter, propertyInfo);
        var conversion = Expression.Convert(propertyAccess, typeof(object));
        var keySelector = Expression.Lambda<Func<T, object>>(conversion, parameter).Compile();

        return isAscending == false
            ? source.OrderByDescending(keySelector)
            : source.OrderBy(keySelector);
    }

    #endregion
}