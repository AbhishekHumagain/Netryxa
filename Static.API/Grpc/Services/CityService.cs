using Static.Common;
using Static.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using City = Static.Protos.City;

namespace Static.API.Grpc.Services;

/// <summary>
/// 
/// </summary>
public class CityService: City.CityBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<GetHealthStatusResponse> GetHealthStatus(Empty request, ServerCallContext context) 
        => Task.FromResult(new GetHealthStatusResponse()
        {
            Status = HealthStatus.Healthy.ToString(),
        });

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<CityDetailMessage> GetCityDetails(IdRequest request, ServerCallContext context)
    {
        return Task.FromResult(new CityDetailMessage
        {
            Name= "Hello " + request.Id
        });
    }
}