using Static.EntityFrameworkCore.Contracts.Entity;

namespace Static.EntityFrameworkCore.Contracts.Auditing
{
    /// <summary>
    /// Represents a base contract for auditable entities with a primary key and audit key.
    /// </summary>
    /// <typeparam name="TEntityKey">The type of the entity's primary key.</typeparam>
    /// <typeparam name="TAuditKey">The type of the audit key.</typeparam>
    public interface IAuditableEntity<TEntityKey, TAuditKey> :
        IEntity<TEntityKey>,
        IAuditable<TAuditKey>
        where TEntityKey : struct, IEquatable<TEntityKey>
        where TAuditKey : struct, IEquatable<TAuditKey>;

    /// <summary>
    /// Represents a base contract for auditable entities with a primary key, a user entity, and a user key.
    /// </summary>
    /// <typeparam name="TEntityKey">The type of the entity's primary key.</typeparam>
    /// <typeparam name="TUser">The type of the user entity associated with the audit.</typeparam>
    /// <typeparam name="TAuditKey">The type of the user entity's primary key.</typeparam>
    public interface IAuditableEntity<TEntityKey, TUser, TAuditKey> :
        IAuditable<TUser, TAuditKey>,
        IAuditableEntity<TEntityKey, TAuditKey>
        where TEntityKey : struct, IEquatable<TEntityKey>
        where TAuditKey : struct, IEquatable<TAuditKey>
        where TUser : class, IEntity<TAuditKey>;

    namespace Default
    {
        /// <summary>
        /// Represents a base contract for auditable entities with default primary and audit keys as <see cref="Guid"/>.
        /// </summary>
        public interface IAuditableEntity :
            IAuditable,
            IAuditableEntity<Guid, Guid>,
            IEntity;

        /// <summary>
        /// Represents a base contract for auditable entities with a user entity and default keys as <see cref="Guid"/>.
        /// </summary>
        /// <typeparam name="TUser">The type of the user entity associated with the audit.</typeparam>
        public interface IAuditableEntity<TUser> :
            IAuditable<TUser>,
            IAuditableEntity,
            IAuditableEntity<Guid, TUser, Guid>
            where TUser : class, IEntity, IEntity<Guid>;
    }
}