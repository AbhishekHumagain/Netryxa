namespace Static.EntityFrameworkCore.Contracts.Auditing;

/// <summary>
/// Represents an entity that tracks its deletion time and supports soft deletion.
/// </summary>
public interface IHasDeletionTime : ISoftDeletable, IAuditKind
{
    /// <summary>
    /// Gets or sets the date and time when the entity was deleted, or <c>null</c> if the entity has not been deleted.
    /// </summary>
    DateTime? DeletionTime { get; set; }
}