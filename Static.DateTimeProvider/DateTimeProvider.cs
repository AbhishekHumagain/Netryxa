namespace Static.DateTimeProvider;

/// <summary>
/// Default implementation of <see cref="IDateTimeProvider"/> 
/// that provides the system's current date and time.
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{

    /// <inheritdoc />
    public DateTime Now => DateTime.UtcNow;
    public DateTime LocalNow => DateTime.Now;

    /// <inheritdoc />
    public DateTime Today => DateTime.Today;

    /// <inheritdoc />
    public DateTimeOffset NowOffset => DateTimeOffset.UtcNow;
    public DateTimeOffset LocalNowOffset => DateTimeOffset.Now;
}