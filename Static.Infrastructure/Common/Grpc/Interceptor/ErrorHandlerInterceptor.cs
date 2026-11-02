using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Static.Infrastructure.Common.Grpc.Interceptor;

// Todo might be uncecessary, because RpcExceptionhandler does the work.
public class ErrorHandlerInterceptor : global::Grpc.Core.Interceptors.Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var call = continuation(request, context);

        return new(
            HandleResponse(call.ResponseAsync),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    private static async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> inner) => await inner;
}