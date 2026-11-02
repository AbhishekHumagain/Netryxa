namespace Static.DateTimeProvider;

/// <summary>
/// Provides an abstraction for obtaining the current date and time.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current local date and time.
    /// </summary>
    DateTime Now { get; }

    DateTime LocalNow { get; }
    
    /// <summary>
    /// Gets the current date with the time set to midnight.
    /// </summary>
    DateTime Today { get; }

    /// <summary>
    /// Gets the current local date and time with offset.
    /// </summary>
    DateTimeOffset NowOffset { get; }
    
    DateTimeOffset LocalNowOffset { get; }
}