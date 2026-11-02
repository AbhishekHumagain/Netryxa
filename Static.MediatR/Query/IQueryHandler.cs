using Static.Results;
using MediatR;

namespace Static.MediatR.Query;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
    
public interface IQueryHandler <TQuery>: IRequestHandler<TQuery, Result>
    where TQuery : IQuery
{
}