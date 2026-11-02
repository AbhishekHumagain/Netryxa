using Static.EntityFrameworkCore.UnitOfWork;
using Static.Results;
using Static.UserSession.CurrentUser.Guid;
using Microsoft.Extensions.Logging;

namespace Static.Application.Common.Behaviours;

public sealed class UnitOfWorkBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger) : MediatR.Behaviours.UnitOfWork.UnitOfWorkBehavior<TRequest, TResponse>(currentUserService, logger) 
    where TRequest : notnull 
    where  TResponse : IResult
{
    protected override Task CommitTransactionAsync(CancellationToken cancellationToken) => unitOfWork.CommitAsync(cancellationToken);
}

