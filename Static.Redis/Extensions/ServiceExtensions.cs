using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Static.Redis.Service;

namespace Static.Redis.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddStaticRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");
        ArgumentException.ThrowIfNullOrEmpty(redisConnectionString);
        
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
    
}