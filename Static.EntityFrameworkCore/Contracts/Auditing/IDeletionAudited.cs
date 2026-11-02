using Static.EntityFrameworkCore.Contracts.Entity;

namespace Static.EntityFrameworkCore.Contracts.Auditing;

/// <summary>
/// Represents an entity that tracks the deletion time and the deleter's user ID.
/// </summary>
/// <typeparam name="TPrimaryKey">The type of the primary key for the deleter user.</typeparam>
public interface IDeletionAudited<TPrimaryKey> : IHasDeletionTime
    where TPrimaryKey : struct, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// Gets or sets the primary key of the user who deleted the entity.
    /// </summary>
    TPrimaryKey? DeleterUserId { get; set; }
}

/// <summary>
/// Represents an entity that tracks the deletion time and the deleter user, along with their ID.
/// </summary>
/// <typeparam name="TUser">The type of the user entity associated with the deleter.</typeparam>
/// <typeparam name="TPrimaryKey">The type of the primary key for the deleter user.</typeparam>
public interface IDeletionAudited<TUser, TPrimaryKey> : IDeletionAudited<TPrimaryKey>
    where TPrimaryKey : struct, IEquatable<TPrimaryKey>
    where TUser : class, IEntity<TPrimaryKey>
{
    /// <summary>
    /// Gets or sets the user entity that deleted the entity.
    /// </summary>
    TUser? DeleterUser { get; set; }
}

/// <summary>
/// Represents an entity that tracks the deletion time and the deleter's user ID using <see cref="Guid"/> as the key type.
/// </summary>
public interface IDeletionAudited : IDeletionAudited<Guid>;