using Static.Results;
using Static.UserSession.CurrentUser.Service;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Static.MediatR.Behaviours.Performance;

public class PerformanceBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger,
    ICurrentUserService currentUserService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
    public virtual async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var startTimeStamp = Stopwatch.GetTimestamp();

        var response = await next(cancellationToken);

        var elapsedTime = Stopwatch.GetElapsedTime(startTimeStamp);

        var elapsedMilliseconds = elapsedTime.TotalMilliseconds;

        if (elapsedMilliseconds <= 500) return response;

        var requestName = typeof(TRequest).Name;
        var userId = currentUserService.UserId;

        logger.LogWarning("[Long Running Request] {RequestName} from user with Id: {UserId} took {ElapsedMilliseconds}ms.", requestName, userId, elapsedMilliseconds);

        return response;
    }
}