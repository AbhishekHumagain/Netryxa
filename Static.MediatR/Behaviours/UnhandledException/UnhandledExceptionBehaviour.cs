using Static.Results;
using Static.UserSession.CurrentUser.Service;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Static.MediatR.Behaviours.UnhandledException;

public class UnhandledExceptionBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger,
    ICurrentUserService currentUserService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
    public virtual async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (FluentValidation.ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Unhandled Exception] {RequestName} for UserId: {UserId} - Error: {ErrorMessage}",
                typeof(TRequest).Name,
                currentUserService.UserId,
                ex.Message);

            throw;
        }
    }
}