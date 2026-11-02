namespace Static.EntityFrameworkCore.Contracts.Auditing;

/// <summary>
/// Represents an entity that tracks the last modification time.
/// </summary>
public interface IHasModificationTime : IAuditKind
{
    /// <summary>
    /// Gets or sets the date and time when the entity was last modified, or <c>null</c> if the entity has not been modified.
    /// </summary>
    DateTime? LastModificationTime { get; set; }
}