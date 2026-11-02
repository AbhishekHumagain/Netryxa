using Static.EntityFrameworkCore.Contracts.Auditing;
using Static.EntityFrameworkCore.Contracts.Auditing.Default;
using Static.EntityFrameworkCore.Contracts.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Static.EntityFrameworkCore.Contracts
{
    /// <summary>
    /// Represents a base class for entities that support auditing for creation, modification, and deletion operations.
    /// </summary>
    /// <typeparam name="TAuditKey">The type of the key used for auditing, typically a struct such as <see cref="Guid"/>.</typeparam>
    public abstract class BaseAuditable<TAuditKey> : Auditing.IAuditable<TAuditKey>
        where TAuditKey : struct, IEquatable<TAuditKey>
    {
        /// <summary>
        /// Gets or sets the timestamp when the entity was created.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created the entity.
        /// </summary>
        public virtual TAuditKey? CreatorUserId { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the entity was last modified.
        /// </summary>
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last modified the entity.
        /// </summary>
        public virtual TAuditKey? LastModifierUserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is deleted.
        /// </summary>
        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the entity was deleted.
        /// </summary>
        public virtual DateTime? DeletionTime { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who deleted the entity.
        /// </summary>
        public virtual TAuditKey? DeleterUserId { get; set; }
    }

    /// <summary>
    /// Represents a base class for entities that support auditing for creation, modification, and deletion operations,
    /// including the user details for these actions.
    /// </summary>
    /// <typeparam name="TUser">The type of the user associated with the auditing operations.</typeparam>
    /// <typeparam name="TAuditKey">The type of the key used for auditing, typically a struct such as <see cref="Guid"/>.</typeparam>
    public abstract class BaseAuditable<TUser, TAuditKey> : BaseAuditable<TAuditKey>, IAuditable<TUser, TAuditKey>
        where TAuditKey : struct, IEquatable<TAuditKey>
        where TUser : class, IEntity<TAuditKey>
    {
        /// <summary>
        /// Gets or sets the user who created the entity.
        /// </summary>
        [ForeignKey(nameof(CreatorUserId))]
        public virtual TUser? CreatorUser { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified the entity.
        /// </summary>
        [ForeignKey(nameof(LastModifierUserId))]
        public virtual TUser? LastModifierUser { get; set; }

        /// <summary>
        /// Gets or sets the user who deleted the entity.
        /// </summary>
        [ForeignKey(nameof(DeleterUserId))]
        public virtual TUser? DeleterUser { get; set; }
    }

    namespace Default
    {
        /// <summary>
        /// Represents a base class for entities that support auditing with default key type of <see cref="Guid"/>
        /// and no user-specific auditing.
        /// </summary>
        public abstract class BaseAuditable : Contracts.BaseAuditable<Guid>, IAuditable;

        /// <summary>
        /// Represents a base class for entities that support auditing with default key type of <see cref="Guid"/>,
        /// including user-specific auditing.
        /// </summary>
        /// <typeparam name="TUser">The type of the user associated with the auditing operations.</typeparam>
        public abstract class BaseAuditable<TUser> : BaseAuditable<TUser, Guid>, Auditing.Default.IAuditable<TUser>
            where TUser : class, IEntity, IEntity<Guid>;
    }
}