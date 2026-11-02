using System.Diagnostics;
using MediatR;
using Static.Results;

namespace Static.MediatR.Behaviours.Tracing;

public static class RequestTracing
{
    public const string SourceName = "Static.MediatR";
    public static readonly ActivitySource Source = new(SourceName);
}

public class TracingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using var activity = RequestTracing.Source.StartActivity(typeof(TRequest).Name, ActivityKind.Internal);
        activity?.SetTag("request.type", typeof(TRequest).FullName);
        try
        {
            var response = await next(cancellationToken);
            activity?.SetStatus(response.IsSuccess ? ActivityStatusCode.Ok : ActivityStatusCode.Error);
            return response;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            activity?.SetTag("request.cancelled", true);
            throw;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.SetTag("error.type", ex.GetType().FullName);
            throw;
        }
    }
}
