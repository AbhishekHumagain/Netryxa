using Microsoft.EntityFrameworkCore.ChangeTracking;
using Static.EntityFrameworkCore.Contracts.Entity;

namespace Static.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for the <see cref="ChangeTracker"/> to interact with tracked entities.
/// </summary>
public static class ChangeTrackerExtensions
{
    /// <summary>
    /// Retrieves an entity from the <see cref="ChangeTracker"/> by its primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to retrieve.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key for the entity.</typeparam>
    /// <param name="changeTracker">The <see cref="ChangeTracker"/> instance that tracks entity changes.</param>
    /// <param name="id">The primary key of the entity to retrieve.</param>
    /// <returns>
    /// The entity of type <typeparamref name="TEntity"/> if found in the change tracker, otherwise <c>null</c>.
    /// </returns>
    public static TEntity? GetFromChangeTracker<TEntity, TPrimaryKey>(this ChangeTracker changeTracker, TPrimaryKey id)
        where TPrimaryKey : struct, IEquatable<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        var entry = changeTracker.Entries()
                                 .FirstOrDefault(
                                     ent =>
                                         ent.Entity is TEntity entity && entity.Id.Equals(id)
                                 );

        return entry?.Entity as TEntity;
    }
}