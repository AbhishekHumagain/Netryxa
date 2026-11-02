using Microsoft.Extensions.Caching.Distributed;

namespace Static.MediatR.Behaviours.Cache;

public static class CacheExpirationProfiles
{
    /// <summary>
    /// Default hybrid strategy: 
    /// - Sliding expiration keeps frequently accessed data alive.
    /// - Absolute expiration guarantees eventual refresh.
    /// </summary>
    public static DistributedCacheEntryOptions Default => new DistributedCacheEntryOptions()
                                                          .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                                                          .SetAbsoluteExpiration(TimeSpan.FromHours(1));

    /// <summary>
    /// Near-realtime data (e.g., live notifications, stock tickers).
    /// Absolute expiration ensures frequent refresh regardless of access patterns.
    /// </summary>
    public static DistributedCacheEntryOptions NearRealtimeData => new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromSeconds(5));

    /// <summary>
    /// Rapidly changing data (e.g., announcements, leaderboards).
    /// Forces refresh every 2 minutes even if frequently accessed.
    /// </summary>
    public static DistributedCacheEntryOptions RapidlyChangingData => new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

    /// <summary>
    /// Static reference data (e.g., dropdowns, country lists).
    /// Retains data for 1 hour after last access, but refreshes daily.
    /// </summary>
    public static DistributedCacheEntryOptions StaticReferenceData => new DistributedCacheEntryOptions()
                                                                      .SetSlidingExpiration(TimeSpan.FromHours(1))
                                                                      .SetAbsoluteExpiration(TimeSpan.FromHours(24));

    /// <summary>
    /// High-traffic hybrid data (e.g., product catalogs, user sessions).
    /// Resets expiration on access, but guarantees refresh after 1 hour.
    /// </summary>
    public static DistributedCacheEntryOptions HighTrafficHybrid => new DistributedCacheEntryOptions()
                                                                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                                                                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

    /// <summary>
    /// Sensitive data (e.g., auth tokens, payment details).
    /// Short-lived with manual invalidation capabilities.
    /// </summary>
    public static DistributedCacheEntryOptions SensitiveData => new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

    /// <summary>
    /// Large infrequently accessed data (e.g., generated reports).
    /// Avoids re-computation costs for 12 hours.
    /// </summary>
    public static DistributedCacheEntryOptions LargeInfrequentData => new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromHours(12));

    /// <summary>
    /// Event-driven data (e.g., live chat, auction bids).
    /// Combines short sliding window with absolute safety net.
    /// </summary>
    public static DistributedCacheEntryOptions EventDrivenData => new DistributedCacheEntryOptions()
                                                                  .SetSlidingExpiration(TimeSpan.FromMinutes(1))
                                                                  .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

    /// <summary>
    /// User-specific preferences (e.g., themes, settings).
    /// Retains data during active use with long-term fallback.
    /// </summary>
    public static DistributedCacheEntryOptions UserPreferences => new DistributedCacheEntryOptions()
                                                                  .SetSlidingExpiration(TimeSpan.FromHours(4))
                                                                  .SetAbsoluteExpiration(TimeSpan.FromDays(7));
}