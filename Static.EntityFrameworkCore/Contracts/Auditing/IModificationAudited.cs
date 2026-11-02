using Static.EntityFrameworkCore.Contracts.Entity;

namespace Static.EntityFrameworkCore.Contracts.Auditing;

/// <summary>
/// Represents an entity that tracks the modification time and the ID of the user who last modified the entity.
/// </summary>
/// <typeparam name="TPrimaryKey">The type of the primary key for the last modifier user.</typeparam>
public interface IModificationAudited<TPrimaryKey> : IHasModificationTime
    where TPrimaryKey : struct, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// Gets or sets the primary key of the user who last modified the entity.
    /// </summary>
    TPrimaryKey? LastModifierUserId { get; set; }
}

/// <summary>
/// Represents an entity that tracks the modification time and the user who last modified the entity, along with their ID.
/// </summary>
/// <typeparam name="TUser">The type of the user entity associated with the modifier.</typeparam>
/// <typeparam name="TPrimaryKey">The type of the primary key for the last modifier user.</typeparam>
public interface IModificationAudited<TUser, TPrimaryKey> : IModificationAudited<TPrimaryKey>
    where TPrimaryKey : struct, IEquatable<TPrimaryKey>
    where TUser : class, IEntity<TPrimaryKey>
{
    /// <summary>
    /// Gets or sets the user entity who last modified the entity.
    /// </summary>
    TUser? LastModifierUser { get; set; }
}

/// <summary>
/// Represents an entity that tracks the modification time and the last modifier's user ID using <see cref="Guid"/> as the key type.
/// </summary>
public interface IModificationAudited : IModificationAudited<Guid>;