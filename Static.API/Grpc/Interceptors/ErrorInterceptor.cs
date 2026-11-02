using Static.Common;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Static.API.Grpc.Interceptors;

/// <summary>
/// 
/// </summary>
public class ErrorInterceptor : Interceptor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <param name="continuation"></param>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            var response = await continuation(request, context);
            var successMessageProperty = response.GetType().GetProperty("StatusResponse");
            if (successMessageProperty == null) return response;
            var successMessage = new StatusResponse { Success = true, Message = "Successfully Called GRPC method" };
            successMessageProperty.SetValue(response, successMessage);
            return response;
        }
        catch (Exception ex)
        {
            return CreateErrorResponse<TResponse>(ex);
        }
    }

    private static TResponse CreateErrorResponse<TResponse>(Exception exception)
    {
        var responseType = typeof(TResponse);
        var responseInstance = Activator.CreateInstance(responseType);
        var errorMessage = new StatusResponse { Success = false, Message = exception.Message };
        var messageProperty = responseType.GetProperty("StatusResponse");
        if (messageProperty == null)
        {
            throw new RpcException(new(StatusCode.Internal, errorMessage.Message));
        }

        messageProperty.SetValue(responseInstance, errorMessage);
        return ((TResponse)responseInstance!)!;
    }
}