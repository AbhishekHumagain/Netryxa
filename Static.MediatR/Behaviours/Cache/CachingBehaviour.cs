using Static.Helper.JsonSerializer;
using Static.Results;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Static.MediatR.Behaviours.Cache.Contracts;
using Static.MediatR.Behaviours.Cache.KeyGenerator;
using Static.MediatR.Query;

namespace Static.MediatR.Behaviours.Cache;

public class CachingBehaviour<TRequest, TResponse>(
    IDistributedCache cache,
    ILogger<CachingBehaviour<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseQuery, ICacheable
    where TResponse : IResult
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestTypeName = typeof(TRequest).Name;
        logger.LogInformation("[Caching Behaviour] Handling request of type: {RequestType}", requestTypeName);

        // Never infer a shared cache key from HTTP state: nested requests and users can collide.
        // Cacheable queries must supply a key that includes user/tenant scope and all query inputs.
        if (string.IsNullOrWhiteSpace(request.CacheKey)) return await next(cancellationToken);
        var cacheKey = request.CacheKey;

        logger.LogInformation(
            "[Caching Behaviour] Attempting to retrieve cache for key: {CacheKey} for request type: {RequestType}",
            cacheKey, requestTypeName);
        var cachedBytes = await cache.GetAsync(cacheKey, cancellationToken);

        if (cachedBytes != null)
        {
            var response = cachedBytes.Deserialize<TResponse>();
            logger.LogInformation("[Caching Behaviour] Cache hit for key: {CacheKey} for request type: {RequestType}",
                cacheKey, requestTypeName);
            if (response != null)
            {
                await cache.RefreshAsync(cacheKey, cancellationToken);
                logger.LogInformation(
                    "[Caching Behaviour] Cache refreshed for key: {CacheKey} for request type: {RequestType}", cacheKey,
                    requestTypeName);
                return response;
            }

            logger.LogWarning(
                "[Caching Behaviour] Cache entry exists but failed to deserialize for key: {CacheKey} for request type: {RequestType}",
                cacheKey, requestTypeName);
        }
        else
        {
            logger.LogInformation("[Caching Behaviour] Cache miss for key: {CacheKey} for request type: {RequestType}",
                cacheKey, requestTypeName);
        }

        // Execute delegate, cache result if successful, and return the result.
        return await ExecuteAndCacheResultAsync(request, next, cacheKey, requestTypeName, cancellationToken);
    }

    /// <summary>
    /// Executes the pipeline delegate and caches the result if successful.
    /// </summary>
    private async Task<TResponse> ExecuteAndCacheResultAsync(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        string cacheKey,
        string requestTypeName,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Caching Behaviour] Executing pipeline delegate for key: {CacheKey} for request type: {RequestType}",
            cacheKey, requestTypeName);
        var response = await next(cancellationToken);

        if (!response.IsSuccess)
        {
            logger.LogInformation(
                "[Caching Behaviour] Response was not successful for key: {CacheKey} for request type: {RequestType}. Not caching result.",
                cacheKey, requestTypeName);
            return response;
        }

        var cacheEntryOptions = request.CacheEntryOptions ?? CacheExpirationProfiles.NearRealtimeData;

        logger.LogInformation(
            "[Caching Behaviour] Caching result for key: {CacheKey} for request type: {RequestType} with sliding expiration {Sliding} seconds and absolute expiration {Absolute} seconds",
            cacheKey, requestTypeName, cacheEntryOptions.SlidingExpiration?.Seconds,
            cacheEntryOptions.AbsoluteExpirationRelativeToNow?.Seconds);

        var serializedData = response.SerializeToUtf8Bytes();
        await cache.SetAsync(cacheKey, serializedData, cacheEntryOptions, cancellationToken);
        logger.LogInformation(
            "[Caching Behaviour] Successfully cached result for key: {CacheKey} for request type: {RequestType}",
            cacheKey, requestTypeName);

        return response;
    }
}