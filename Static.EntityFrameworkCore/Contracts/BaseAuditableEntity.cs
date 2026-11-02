using Static.EntityFrameworkCore.Contracts.Auditing;
using Static.EntityFrameworkCore.Contracts.Auditing.Default;
using Static.EntityFrameworkCore.Contracts.Entity;
using System.ComponentModel.DataAnnotations;

namespace Static.EntityFrameworkCore.Contracts
{
    /// <summary>
    /// Represents a base class for auditable entities with a primary key and auditing fields for creation, modification, and deletion operations.
    /// </summary>
    /// <typeparam name="TEntityKey">The type of the primary key for the entity, typically a struct such as <see cref="Guid"/>.</typeparam>
    /// <typeparam name="TAuditKey">The type of the key used for auditing, typically a struct such as <see cref="Guid"/>.</typeparam>
    public abstract class BaseAuditableEntity<TEntityKey, TAuditKey> : BaseAuditable<TAuditKey>, IAuditableEntity<TEntityKey, TAuditKey>
        where TEntityKey : struct, IEquatable<TEntityKey>
        where TAuditKey : struct, IEquatable<TAuditKey>
    {
        /// <summary>
        /// Gets or sets the primary key of the entity.
        /// </summary>
        [Key]
        public virtual TEntityKey Id { get; set; }
    }

    /// <summary>
    /// Represents a base class for auditable entities with a primary key and user-specific auditing fields.
    /// </summary>
    /// <typeparam name="TEntityKey">The type of the primary key for the entity, typically a struct such as <see cref="Guid"/>.</typeparam>
    /// <typeparam name="TUser">The type of the user associated with the auditing operations.</typeparam>
    /// <typeparam name="TAuditKey">The type of the key used for auditing, typically a struct such as <see cref="Guid"/>.</typeparam>
    public abstract class BaseAuditableEntity<TEntityKey, TUser, TAuditKey> : BaseAuditable<TUser, TAuditKey>, IAuditableEntity<TEntityKey, TUser, TAuditKey>
        where TEntityKey : struct, IEquatable<TEntityKey>
        where TAuditKey : struct, IEquatable<TAuditKey>
        where TUser : class, IEntity<TAuditKey>
    {
        /// <summary>
        /// Gets or sets the primary key of the entity.
        /// </summary>
        [Key]
        public virtual TEntityKey Id { get; set; }
    }

    namespace Default
    {
        /// <summary>
        /// Represents a base class for auditable entities with a primary key and default key type of <see cref="Guid"/> for auditing.
        /// </summary>
        public abstract class BaseAuditableEntity : BaseAuditable, IAuditableEntity
        {
            /// <summary>
            /// Gets or sets the primary key of the entity.
            /// The default value is a new <see cref="Guid"/>.
            /// </summary>
            [Key]
            public virtual Guid Id { get; set; } = Guid.NewGuid();
        }

        /// <summary>
        /// Represents a base class for auditable entities with a primary key, default key type of <see cref="Guid"/>,
        /// and user-specific auditing fields.
        /// </summary>
        /// <typeparam name="TUser">The type of the user associated with the auditing operations.</typeparam>
        public abstract class BaseAuditableEntity<TUser> : BaseAuditable<TUser>, IAuditableEntity<TUser>
            where TUser : class, IEntity, IEntity<Guid>
        {
            /// <summary>
            /// Gets or sets the primary key of the entity.
            /// The default value is a new <see cref="Guid"/>.
            /// </summary>
            [Key]
            public virtual Guid Id { get; set; } = Guid.NewGuid();
        }
    }
}