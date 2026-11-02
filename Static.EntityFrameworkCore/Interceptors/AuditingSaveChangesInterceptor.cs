using Static.DateTimeProvider;
using Static.EntityFrameworkCore.Extensions;
using Static.UserSession.CurrentUser.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Static.EntityFrameworkCore.Contracts.Auditing;

namespace Static.EntityFrameworkCore.Interceptors
{
    /// <summary>
    /// Interceptor that applies auditing logic to entity changes before they are saved to the database.
    /// It handles the creation, modification, and deletion of entities by updating the relevant audit fields.
    /// </summary>
    /// <typeparam name="TUserKey">The type of the user identifier (e.g., Guid or int).</typeparam>
    public class AuditingSaveChangesInterceptor<TUserKey> : SaveChangesInterceptor
        where TUserKey : struct, IEquatable<TUserKey>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService<TUserKey> _currentUserService;

        public AuditingSaveChangesInterceptor(IDateTimeProvider dateTimeProvider, ICurrentUserService<TUserKey> currentUserService)
        {
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Synchronously intercepts changes being saved to the database and applies the audit trail logic.
        /// </summary>
        /// <param name="eventData">Event data for the save operation.</param>
        /// <param name="result">The result of the save operation.</param>
        /// <returns>The result after applying the audit logic.</returns>
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context == null)
            {
                return base.SavingChanges(eventData, result);
            }

            ApplyAuditTrail(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        /// <summary>
        /// Asynchronously intercepts changes being saved to the database and applies the audit trail logic.
        /// </summary>
        /// <param name="eventData">Event data for the save operation.</param>
        /// <param name="result">The result of the save operation.</param>
        /// <param name="cancellationToken">Cancellation token for asynchronous operation.</param>
        /// <returns>The result after applying the audit logic asynchronously.</returns>
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context == null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            ApplyAuditTrail(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        /// <summary>
        /// Applies audit trail logic to the entities being tracked by the DbContext.
        /// Updates creation, modification, and deletion timestamps and user information.
        /// </summary>
        /// <param name="context">The DbContext instance tracking the changes.</param>
        private void ApplyAuditTrail(DbContext context)
        {
            var currentUserId = _currentUserService.UserIdOrNull;

            // Loop through all entities that are auditable
            foreach (var entry in context.ChangeTracker
                                         .Entries<IAuditKind>()
                                         .Where(e => e.State is not (EntityState.Detached or EntityState.Unchanged)))
            {
                var entity = entry.Entity;
                var state = entry.State;

                #region EntityState.Added

                if (state == EntityState.Added)
                {
                    if (entity is IHasCreationTime hasCreationTime)
                    {
                        hasCreationTime.CreationTime = _dateTimeProvider.Now;
                    }

                    if (entity is ICreationAudited<TUserKey> creationAudited)
                    {
                        creationAudited.CreatorUserId = currentUserId;
                    }
                }

                #endregion

                #region EntityState.Modified

                if (state == EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    if (entity is IHasModificationTime hasModificationTime)
                    {
                        hasModificationTime.LastModificationTime = _dateTimeProvider.Now;
                    }

                    if (entity is IModificationAudited<TUserKey> modificationAudited)
                    {
                        modificationAudited.LastModifierUserId = currentUserId;
                    }
                }

                #endregion

                #region EntityState.Deleted

                if (state == EntityState.Deleted)
                {
                    if (entity is IHasDeletionTime hasDeletionTime)
                    {
                        hasDeletionTime.DeletionTime = _dateTimeProvider.Now;
                        hasDeletionTime.IsDeleted = true;
                    }

                    if (entity is IDeletionAudited<TUserKey> deletionAudited)
                    {
                        deletionAudited.DeleterUserId = currentUserId;
                    }

                    entry.State = EntityState.Modified; // Soft delete, marking as modified instead of deleted.
                }

                #endregion
            }
        }
    }
}