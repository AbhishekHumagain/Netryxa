namespace Static.MediatR.Query;

public interface IEnumerableQueryHandler<in TQuery, TResponse> : IQueryHandler<TQuery, IEnumerable<TResponse>>
    where TQuery : IEnumerableQuery<TResponse>;