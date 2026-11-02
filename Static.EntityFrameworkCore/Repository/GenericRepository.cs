using Static.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Static.EntityFrameworkCore.Contracts.Entity;
using System.Linq.Expressions;

namespace Static.EntityFrameworkCore.Repository;

/// <summary>
/// A base generic repository implementation for managing entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public class GenericRepository<TEntity, TPrimaryKey, TDbContext>(TDbContext context) : IGenericRepository<TEntity, TPrimaryKey>
    where TPrimaryKey : struct, IEquatable<TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
    where TDbContext : DbContext
{
    private DbSet<TEntity> DbSet => context.Set<TEntity>();

    /// <summary>
    /// Retrieves all entities.
    /// </summary>
    public virtual IQueryable<TEntity> GetAll() => DbSet;

    /// <summary>
    /// Retrieves all entities with no tracking.
    /// </summary>
    public virtual IQueryable<TEntity> GetAllNoTracking() => DbSet.AsNoTracking();

    /// <summary>
    /// Asynchronously retrieves entities with optional filtering, sorting, pagination, and projection.
    /// </summary>
    public virtual async Task<(IEnumerable<TProjectedType> Items, int TotalCount)> GetAllAsync<TProjectedType>(
        (int Page, int Size) pagination,
        bool calculateTotalCount = true,
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>[]? includes = null,
        (bool IsAscending, Expression<Func<TEntity, object>> KeySelector)[]? orderBy = null,
        Expression<Func<TEntity, TProjectedType>>? selector = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        var noPagination = pagination is { Page: 0, Size: 0 };
        if (!noPagination)
        {
            if (pagination.Page < 0)
            {
                throw new ArgumentException("Page must be greater than 0");
            }

            if (pagination.Size < 0)
            {
                throw new ArgumentException("Size must be greater than 0");
            }
        }

        var query = asNoTracking ? GetAllNoTracking() : GetAll();
        if (filter != null) query = query.Where(filter);
        if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));
        if (orderBy != null && orderBy.Length != 0)
        {
            IOrderedQueryable<TEntity> orderedQuery = null!;
            for (var i = 0; i < orderBy.Length; i++)
            {
                var (isAscending, keySelector) = orderBy[i];
                orderedQuery = i == 0
                    ? isAscending ? query.OrderBy(keySelector) : query.OrderByDescending(keySelector)
                    : isAscending
                        ? orderedQuery.ThenBy(keySelector)
                        : orderedQuery.ThenByDescending(keySelector);
            }

            query = orderedQuery;
        }

        var totalCount = calculateTotalCount ? await query.CountAsync(cancellationToken) : 0;
        query = noPagination
            ? query
            : query.Skip((pagination.Page - 1) * pagination.Size)
                   .Take(pagination.Size);

        var items = selector != null
            ? await query.Select(selector).ToArrayAsync(cancellationToken)
            : await query.Cast<TProjectedType>().ToArrayAsync(cancellationToken);
        return (Items: items, TotalCount: totalCount);
    }

    public virtual async Task<IEnumerable<TProjectedType>> GetAllAsync<TProjectedType>(
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>[]? includes = null,
        (bool IsAscending, Expression<Func<TEntity, object>> KeySelector)[]? orderBy = null,
        Expression<Func<TEntity, TProjectedType>>? selector = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = asNoTracking ? GetAllNoTracking() : GetAll();
        if (filter != null) query = query.Where(filter);
        if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));
        if (orderBy != null && orderBy.Length != 0)
        {
            IOrderedQueryable<TEntity> orderedQuery = null!;
            for (var i = 0; i < orderBy.Length; i++)
            {
                var (isAscending, keySelector) = orderBy[i];
                orderedQuery = i == 0
                    ? isAscending ? query.OrderBy(keySelector) : query.OrderByDescending(keySelector)
                    : isAscending
                        ? orderedQuery.ThenBy(keySelector)
                        : orderedQuery.ThenByDescending(keySelector);
            }

            query = orderedQuery;
        }

        var items = selector != null
            ? await query.Select(selector).ToArrayAsync(cancellationToken)
            : await query.Cast<TProjectedType>().ToArrayAsync(cancellationToken);
        return items;
    }

    public virtual async Task<TProjectedType?> GetAsync<TProjectedType>(
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>[]? includes = null,
        Expression<Func<TEntity, TProjectedType>>? selector = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = asNoTracking ? GetAllNoTracking() : GetAll();
        if (filter != null) query = query.Where(filter);
        if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));
        return selector != null
            ? await query.Select(selector).SingleOrDefaultAsync(cancellationToken)
            : await query.Cast<TProjectedType>().SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously retrieves an entity by its ID, with optional includes and no-tracking.
    /// </summary>
    public virtual Task<TEntity?> GetAsync(TPrimaryKey id, Expression<Func<TEntity, object>>[]? includes = null, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = asNoTracking ? GetAllNoTracking() : GetAll();
        if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));
        return query.SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    /// <summary>
    /// Asynchronously retrieves a single entity matching the predicate.
    /// </summary>
    public virtual Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var ctx = asNoTracking ? GetAllNoTracking() : GetAll();
        return ctx.SingleAsync(predicate, cancellationToken);
    }
    
    /// <summary>
    /// Asynchronously retrieves a single entity matching the predicate or default.
    /// </summary>
    public virtual Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var ctx = asNoTracking ? GetAllNoTracking() : GetAll();
        return ctx.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Asynchronously retrieves the first entity matching the predicate.
    /// </summary>
    public virtual Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var ctx = asNoTracking ? GetAllNoTracking() : GetAll();
        return ctx.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(predicate, cancellationToken);

    public Task<bool> AnyAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(x => x.Id.Equals(id), cancellationToken);

    /// <summary>
    /// Inserts a new entity.
    /// </summary>
    public virtual void Insert(TEntity entity) => DbSet.Add(entity);

    /// <summary>
    /// Asynchronously inserts a new entity and saves changes.
    /// </summary>
    public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    /// <summary>
    /// Asynchronously inserts a new entity and retrieves its ID.
    /// </summary>
    public virtual async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var insertedEntity = await InsertAsync(entity, cancellationToken);
        return insertedEntity.Id;
    }

    /// <summary>
    /// Inserts multiple entities.
    /// </summary>
    public virtual void InsertRange(IEnumerable<TEntity> entities) => DbSet.AddRange(entities);

    /// <summary>
    /// Asynchronously inserts multiple entities and saves changes.
    /// </summary>
    public virtual Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => DbSet.AddRangeAsync(entities, cancellationToken);

    /// <summary>
    /// Asynchronously inserts multiple entities in batches to prevent overload.
    /// </summary>
    public virtual async Task BulkInsertAsync(IEnumerable<TEntity> entities, int batchSize = 100, CancellationToken cancellationToken = default)
    {
        if (!entities.Any()) return;

        foreach (var batch in entities.Chunk(batchSize))
        {
            await DbSet.AddRangeAsync(batch, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    public virtual void Update(TEntity entity)
    {
        context.AttachIfNot(entity);
        DbSet.Update(entity);
    }

    /// <summary>
    /// Asynchronously execute updates in the specified repo table. USE CAUTIOUSLY!
    /// </summary>
    public Task<int> UpdateAsync(
        Action<UpdateSettersBuilder<TEntity>> setPropertyCalls,
        CancellationToken cancellationToken = default)
        => DbSet.ExecuteUpdateAsync(setPropertyCalls, cancellationToken);

    
    /// <summary>
    /// Asynchronously execute updates depending on condition in the specified repo table.
    /// </summary>
    public Task<int> UpdateAsync(
        Expression<Func<TEntity, bool>> condition,
        Action<UpdateSettersBuilder<TEntity>> setPropertyCalls,
        CancellationToken cancellationToken = default)
        => DbSet.Where(condition).ExecuteUpdateAsync(setPropertyCalls, cancellationToken);

    /// <summary>
    /// Updates multiple entities.
    /// </summary>
    public virtual void UpdateRange(IEnumerable<TEntity> entities) => DbSet.UpdateRange(entities);

    /// <summary>
    /// Asynchronously updates multiple entities in batches.
    /// </summary>
    public virtual async Task BulkUpdateAsync(IEnumerable<TEntity> entities, int batchSize = 100, CancellationToken cancellationToken = default)
    {
        if (!entities.Any()) return;

        foreach (var batch in entities.Chunk(batchSize))
        {
            DbSet.UpdateRange(batch);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    public virtual void Delete(TEntity entity)
    {
        context.AttachIfNot(entity);
        DbSet.Remove(entity);
    }

    /// <summary>
    /// Asynchronously performs a hard delete of an entity by executing delete command.
    /// </summary>
    public virtual Task HardDeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        => GetAll().Where(x => x.Id.Equals(entity.Id)).ExecuteDeleteAsync(cancellationToken);

    /// <summary>
    /// Asynchronously deletes an entity by ID.
    /// </summary>
    public virtual async Task DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
    {
        var entity = context.ChangeTracker.GetFromChangeTracker<TEntity, TPrimaryKey>(id) ?? await GetAsync(id, cancellationToken: cancellationToken);
        if (entity != null)
        {
            DbSet.Remove(entity);
        }
    }

    /// <summary>
    /// Asynchronously performs a hard delete of an entity by ID.
    /// </summary>
    public virtual Task HardDeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        => GetAll().Where(x => x.Id.Equals(id)).ExecuteDeleteAsync(cancellationToken);

    /// <summary>
    /// Asynchronously deletes entities matching the predicate.
    /// </summary>
    public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        foreach (var entity in await DbSet.Where(predicate).ToArrayAsync(cancellationToken))
        {
            DbSet.Entry(entity).State = EntityState.Deleted;
        }
    }

    /// <summary>
    /// Asynchronously performs a hard delete of entities matching the predicate.
    /// </summary>
    public virtual Task HardDeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => GetAll().Where(predicate).ExecuteDeleteAsync(cancellationToken);

    /// <summary>
    /// Deletes multiple entities.
    /// </summary>
    public virtual void DeleteRange(IEnumerable<TEntity> entities)
    {
        context.AttachIfNot(entities);
        DbSet.RemoveRange(entities);
    }

    /// <summary>
    /// Asynchronously performs a hard delete of multiple entities.
    /// </summary>
    public virtual Task HardDeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var ids = entities.Select(x => x.Id).ToHashSet();
        return HardDeleteRangeAsync(ids, cancellationToken);
    }

    /// <summary>
    /// Asynchronously performs a hard delete of entities by their IDs.
    /// </summary>
    public virtual Task HardDeleteRangeAsync(IEnumerable<TPrimaryKey> ids, CancellationToken cancellationToken = default)
        => GetAll().Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync(cancellationToken);

    /// <summary>
    /// Asynchronously counts all entities as <see cref="int"/>.
    /// </summary>
    public virtual Task<int> CountAsync(CancellationToken cancellationToken = default) => GetAll().CountAsync(cancellationToken);

    /// <summary>
    /// Asynchronously counts entities as <see cref="int"/>, matching the predicate.
    /// </summary>
    public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => GetAll().CountAsync(predicate, cancellationToken);

    /// <summary>
    /// Asynchronously counts all entities as <see cref="long"/>.
    /// </summary>
    public virtual Task<long> LongCountAsync(CancellationToken cancellationToken = default) => GetAll().LongCountAsync(cancellationToken);

    /// <summary>
    /// Asynchronously counts entities as <see cref="long"/>, matching the predicate.
    /// </summary>
    public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => GetAll().LongCountAsync(predicate, cancellationToken);

    /// <summary>
    /// Executes a raw SQL query against the database and returns an <see cref="IQueryable{TEntity}"/> to enable further query composition.
    /// </summary>
    public virtual IQueryable<TEntity> FromSql(FormattableString sql) => DbSet.FromSql(sql);

    /// <summary>
    /// Executes a raw SQL query against the database and returns an <see cref="IQueryable{TEntity}"/> to enable further query composition.
    /// </summary>
    public virtual IQueryable<TEntity> FromSql(string sql, object[] parameters) => DbSet.FromSqlRaw(sql, parameters);

    /// <summary>
    /// Executes a raw SQL query against the database asynchronously and returns the result as an <see cref="IEnumerable{TEntity}"/>.
    /// </summary>
    public virtual async Task<IEnumerable<TEntity>> FromSqlAsync(FormattableString sql, CancellationToken cancellationToken = default)
        => await FromSql(sql).ToArrayAsync(cancellationToken);

    /// <summary>
    /// Executes a raw SQL query against the database asynchronously and returns the result as an <see cref="IEnumerable{TEntity}"/>.
    /// </summary>
    public virtual async Task<IEnumerable<TEntity>> FromSqlAsync(string sql, object[] parameters, CancellationToken cancellationToken = default)
        => await FromSql(sql, parameters).ToArrayAsync(cancellationToken);
}