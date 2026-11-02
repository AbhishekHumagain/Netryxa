using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json.Serialization;

namespace Static.MediatR.Behaviours.Cache.Contracts;

public interface ICacheable
{
    [JsonIgnore] string? CacheKey { get; }
    [JsonIgnore] DistributedCacheEntryOptions? CacheEntryOptions { get; }
}