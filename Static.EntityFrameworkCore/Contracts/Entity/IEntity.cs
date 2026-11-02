namespace Static.EntityFrameworkCore.Contracts.Entity;

/// <summary>
/// Represents an entity with a primary key of type <typeparamref name="TPrimaryKey"/>.
/// </summary>
/// <typeparam name="TPrimaryKey">The type of the primary key for the entity.</typeparam>
public interface IEntity<TPrimaryKey> : IEntityKind
    where TPrimaryKey : struct
{
    /// <summary>
    /// Gets or sets the primary key of the entity.
    /// </summary>
    TPrimaryKey Id { get; set; }
}

/// <summary>
/// Represents an entity with a primary key of type <see cref="Guid"/>.
/// </summary>
public interface IEntity : IEntity<Guid>;