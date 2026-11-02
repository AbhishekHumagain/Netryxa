using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json.Serialization;

namespace Static.MediatR.Behaviours.Cache.Contracts;

public abstract record BaseCacheable : ICacheable
{
    [JsonIgnore] public string? CacheKey { get; init; }
    [JsonIgnore] public DistributedCacheEntryOptions? CacheEntryOptions { get; init; }
}