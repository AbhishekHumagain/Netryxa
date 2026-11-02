using Static.EntityFrameworkCore.Contracts.Entity;

namespace Static.EntityFrameworkCore.Contracts.Auditing;

/// <summary>
/// Represents an entity that tracks the creation time and the creator's user ID.
/// </summary>
/// <typeparam name="TPrimaryKey">The type of the primary key for the creator user.</typeparam>
public interface ICreationAudited<TPrimaryKey> : IHasCreationTime
    where TPrimaryKey : struct, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// Gets or sets the primary key of the user who created the entity.
    /// </summary>
    TPrimaryKey? CreatorUserId { get; set; }
}

/// <summary>
/// Represents an entity that tracks the creation time and the creator user, along with their ID.
/// </summary>
/// <typeparam name="TUser">The type of the user entity associated with the creator.</typeparam>
/// <typeparam name="TPrimaryKey">The type of the primary key for the creator user.</typeparam>
public interface ICreationAudited<TUser, TPrimaryKey> : ICreationAudited<TPrimaryKey>
    where TPrimaryKey : struct, IEquatable<TPrimaryKey>
    where TUser : class, IEntity<TPrimaryKey>
{
    /// <summary>
    /// Gets or sets the user entity that created the entity.
    /// </summary>
    TUser? CreatorUser { get; set; }
}

/// <summary>
/// Represents an entity that tracks the creation time and the creator's user ID using <see cref="Guid"/> as the key type.
/// </summary>
public interface ICreationAudited : ICreationAudited<Guid>;