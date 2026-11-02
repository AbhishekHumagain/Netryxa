using Static.Helper.Transaction;
using Static.Results;
using Static.UserSession.CurrentUser.Service;
using MediatR;
using Microsoft.Extensions.Logging;
using Static.MediatR.Command;
using Static.MediatR.Query;
using System.Diagnostics.CodeAnalysis;

namespace Static.MediatR.Behaviours.UnitOfWork;

public abstract class UnitOfWorkBehavior<TRequest, TResponse>(
    ICurrentUserService currentUserService,
    ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
    public virtual async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IBaseQuery or INonTransactionalCommand) return await next(cancellationToken);

        using var transactionScope = GlobalTransactionFactory.CreateAsyncScope();

        var requestName = typeof(TRequest).Name;
        TResponse response;
        try
        {
            response = await next(cancellationToken);

            if (IsFailureResponse(response, out var error))
            {
                if (!error.CommitTransaction)
                {
                    return response;
                }
            }

            await CommitTransactionAsync(cancellationToken);
            transactionScope.Complete();

            LogSuccess(requestName);
        }
        catch
        {
            LogError(requestName);
            throw;
        }

        return response;
    }

    private static bool IsFailureResponse(TResponse response, [NotNullWhen(true)] out Error? error)
    {
        if (response.IsSuccess == false)
        {
            error = response.Error;
            return true;
        }

        error = null;
        return false;
    }

    protected abstract Task CommitTransactionAsync(CancellationToken cancellationToken);

    private void LogSuccess(string requestName) => logger.LogInformation("[UoW Transaction] Static UnitOfWork request succeeded for: {RequestName}, UserId = {UserId}.", requestName, currentUserService.UserId);

    private void LogError(string requestName) => logger.LogError("[UoW Transaction] Static UnitOfWork request failed for: {RequestName}, UserId = {UserId}.", requestName, currentUserService.UserId);
}