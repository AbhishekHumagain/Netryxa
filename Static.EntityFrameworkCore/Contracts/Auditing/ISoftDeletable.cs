namespace Static.EntityFrameworkCore.Contracts.Auditing;

/// <summary>
/// Represents an entity that supports soft deletion by marking it as deleted without physically removing it from the database.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is considered deleted.
    /// </summary>
    bool IsDeleted { get; set; }
}