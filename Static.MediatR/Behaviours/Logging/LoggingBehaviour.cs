using Static.Results;
using Static.UserSession.CurrentUser.Service;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Static.MediatR.Behaviours.Logging;

public class LoggingBehaviour<TRequest, TResponse>(
    ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
    ICurrentUserService currentUserService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    public virtual async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("[Request Logging] Static Handling {@RequestName} request for {@UserId}.", requestName, currentUserService.UserId);
        var result = await next(cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("[Request Logging] Static failed handling {@RequestName} request for {@UserId} with {StatusCode}.", requestName, currentUserService.UserId, result.Error?.Code);
        }

        logger.LogInformation("[Request Logging] Static Handled {@RequestName} request for {@UserId}.", requestName, currentUserService.UserId);
        return result;
    }
}