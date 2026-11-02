using Grpc.Health.V1;
using Grpc.Net.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Grpc.Core;
namespace Static.API.Configurations;

/// <summary>
/// This will check health of identity grpc
/// </summary>
public class GrpcServiceHealthCheck(IConfiguration configuration) : IHealthCheck
{
    /// <summary>
    /// Method related to Check Health Async.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        configuration.GetSection("GRPC").Get<GrpcConfigurationGetValue>();
        if (string.IsNullOrEmpty(GrpcConfigurationGetValue.IdentityUrl))
        {
            return HealthCheckResult.Unhealthy("gRPC IdentityUrl is not configured");
        }
        
        try
        {
            using var channel = GrpcChannel.ForAddress(GrpcConfigurationGetValue.IdentityUrl);
            var healthClient = new Health.HealthClient(channel);
            
            var response = await healthClient.CheckAsync(new HealthCheckRequest(), deadline: DateTime.UtcNow.AddSeconds(2));

            return response.Status == HealthCheckResponse.Types.ServingStatus.Serving
                ? HealthCheckResult.Healthy("gRPC Service is healthy")
                : HealthCheckResult.Unhealthy("gRPC Service is not serving");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
        {
            return HealthCheckResult.Unhealthy("gRPC Service timed out");
        }
        catch (RpcException ex)
        {
            return HealthCheckResult.Unhealthy($"gRPC Service error: {ex.Status}");
        }
        catch (Exception)
        {
            return HealthCheckResult.Unhealthy("gRPC Service is unavailable");
        }
    }
}
