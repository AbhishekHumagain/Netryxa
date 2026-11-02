using Static.EntityFrameworkCore.Contracts.Entity;

namespace Static.EntityFrameworkCore.Contracts;

/// <summary>
/// Base class for entities with a primary key of type <typeparamref name="TPrimaryKey"/>.
/// </summary>
/// <typeparam name="TPrimaryKey">The type of the primary key for the entity.</typeparam>
public abstract class BaseEntity<TPrimaryKey> : IEntity<TPrimaryKey>
    where TPrimaryKey : struct, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// Gets or sets the primary key of the entity.
    /// </summary>
    public TPrimaryKey Id { get; set; }
}

/// <summary>
/// Base class for entities with a primary key of type <see cref="Guid"/>.
/// </summary>
public abstract class BaseEntity : BaseEntity<Guid>, IEntity;