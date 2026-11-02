using Microsoft.EntityFrameworkCore;
using Static.EntityFrameworkCore.Contracts.Entity;

namespace Static.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for <see cref="DbContext"/> to attach entities to the context if they are not already attached.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Attaches a collection of entities to the <see cref="DbContext"/> if they are not already attached.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to attach.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance to attach the entities to.</param>
    /// <param name="entities">The collection of entities to attach to the context.</param>
    public static void AttachIfNot<TEntity>(this DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class, IEntityKind
    {
        foreach (var entity in entities)
        {
            AttachIfNot(context, entity);
        }
    }

    /// <summary>
    /// Attaches a single entity to the <see cref="DbContext"/> if it is not already attached.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to attach.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance to attach the entity to.</param>
    /// <param name="entity">The entity to attach to the context.</param>
    public static void AttachIfNot<TEntity>(this DbContext context, TEntity entity)
        where TEntity : class, IEntityKind
    {
        if (context.Entry(entity).State == EntityState.Detached)
        {
            context.Set<TEntity>().Attach(entity);
        }
    }
}