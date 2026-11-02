namespace Static.EntityFrameworkCore.Contracts.Auditing;

/// <summary>
/// Represents an entity that tracks its creation time.
/// </summary>
public interface IHasCreationTime : IAuditKind
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    DateTime CreationTime { get; set; }
}