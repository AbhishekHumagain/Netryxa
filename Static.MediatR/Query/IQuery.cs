using Static.Results;
using MediatR;

namespace Static.MediatR.Query;

public interface IBaseQuery;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>, IBaseQuery;


public interface IQuery : IRequest<Result>, IBaseQuery
{
}