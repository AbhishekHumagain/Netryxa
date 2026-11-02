using Static.EntityFrameworkCore.Contracts.Entity;

namespace Static.EntityFrameworkCore.Contracts.Auditing
{
    /// <summary>
    /// Represents an entity that supports auditing for creation, modification, and deletion operations.
    /// </summary>
    /// <typeparam name="TAuditKey">The type of the key used for auditing, typically a struct such as <see cref="Guid"/>.</typeparam>
    public interface IAuditable<TAuditKey> :
        ICreationAudited<TAuditKey>,
        IModificationAudited<TAuditKey>,
        IDeletionAudited<TAuditKey>
        where TAuditKey : struct, IEquatable<TAuditKey>;

    /// <summary>
    /// Represents an entity that supports detailed auditing for creation, modification, and deletion operations,
    /// including the user who performed these actions.
    /// </summary>
    /// <typeparam name="TUser">The type of the user associated with the auditing operations.</typeparam>
    /// <typeparam name="TAuditKey">The type of the key used for auditing, typically a struct such as <see cref="Guid"/>.</typeparam>
    public interface IAuditable<TUser, TAuditKey> :
        IAuditable<TAuditKey>,
        ICreationAudited<TUser, TAuditKey>,
        IModificationAudited<TUser, TAuditKey>,
        IDeletionAudited<TUser, TAuditKey>
        where TAuditKey : struct, IEquatable<TAuditKey>
        where TUser : class, IEntity<TAuditKey>;

    namespace Default
    {
        /// <summary>
        /// Represents an auditable entity with default key type of <see cref="Guid"/> and no user-specific auditing.
        /// </summary>
        public interface IAuditable :
            Auditing.IAuditable<Guid>,
            ICreationAudited,
            IModificationAudited,
            IDeletionAudited;

        /// <summary>
        /// Represents an auditable entity with default key type of <see cref="Guid"/>, including user-specific auditing.
        /// </summary>
        /// <typeparam name="TUser">The type of the user associated with the auditing operations.</typeparam>
        public interface IAuditable<TUser> :
            IAuditable<TUser, Guid>,
            IAuditable
            where TUser : class, IEntity, IEntity<Guid>;
    }
}