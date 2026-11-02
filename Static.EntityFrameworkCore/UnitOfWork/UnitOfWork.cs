using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Static.EntityFrameworkCore.Contracts.Entity;
using Static.EntityFrameworkCore.Repository;
using IsolationLevel = System.Data.IsolationLevel;

namespace Static.EntityFrameworkCore.UnitOfWork;

/// <summary>
/// Provides a base implementation of the unit of work pattern for managing database transactions, repositories, and SQL execution.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context used by the unit of work.</typeparam>
public class UnitOfWork<TDbContext>(TDbContext dbContext, IServiceProvider serviceProvider) : IUnitOfWork where TDbContext : DbContext
{
    /// <summary>
    /// Commits all changes made within the unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation, with the number of state entries written to the database.</returns>
    public virtual Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);

    /// <summary>
    /// Begins a database transaction with the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">The isolation level for the transaction. Defaults to <see cref="IsolationLevel.Unspecified"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation, with the transaction object.</returns>
    public virtual Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default)
        => dbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);

    public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class => dbContext.Set<TEntity>();

    /// <summary>
    /// Gets a generic repository for managing entities of the specified type with a custom primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
    /// <returns>An instance of <see cref="IGenericRepository{TEntity,TPrimaryKey}"/>.</returns>
    public virtual IGenericRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>()
        where TPrimaryKey : struct, IEquatable<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        => ActivatorUtilities.CreateInstance<GenericRepository<TEntity, TPrimaryKey, TDbContext>>(serviceProvider);

    /// <summary>
    /// Gets a generic repository for managing entities of the specified type with a <see cref="Guid"/> primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>An instance of <see cref="IGenericRepository{TEntity, TDbContext}"/>.</returns>
    public virtual Repository.Guid.IGenericRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class, IEntity<Guid>
        => ActivatorUtilities.CreateInstance<Repository.Guid.GenericRepository<TEntity, TDbContext>>(serviceProvider);

    /// <summary>
    /// Executes a raw SQL query and returns a queryable result set.
    /// </summary>
    /// <typeparam name="TResult">The type of the result set.</typeparam>
    /// <param name="sql">The raw SQL query to execute.</param>
    /// <param name="parameters">An array of parameters to pass to the query.</param>
    /// <returns>An <see cref="IQueryable{T}"/> representing the result set.</returns>
    public virtual IQueryable<TResult> FromSql<TResult>(string sql, object[] parameters)
        => dbContext.Database.SqlQueryRaw<TResult>(sql, parameters);

    /// <summary>
    /// Executes a formatted SQL query and returns a queryable result set.
    /// </summary>
    /// <typeparam name="TResult">The type of the result set.</typeparam>
    /// <param name="sql">The formatted SQL query to execute.</param>
    /// <returns>An <see cref="IQueryable{T}"/> representing the result set.</returns>
    public virtual IQueryable<TResult> FromSql<TResult>(FormattableString sql)
        => dbContext.Database.SqlQuery<TResult>(sql);

    /// <summary>
    /// Executes a raw SQL command asynchronously.
    /// </summary>
    /// <param name="sql">The raw SQL command to execute.</param>
    /// <param name="parameters">An array of parameters to pass to the command.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation, with the number of rows affected.</returns>
    public virtual Task<int> ExecuteSqlAsync(string sql, object[] parameters, CancellationToken cancellationToken = default)
        => dbContext.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);

    /// <summary>
    /// Executes a raw SQL command asynchronously.
    /// </summary>
    /// <param name="sql">The raw SQL command to execute.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation, with the number of rows affected.</returns>
    public virtual Task<int> ExecuteSqlAsync(FormattableString sql, CancellationToken cancellationToken = default)
        => dbContext.Database.ExecuteSqlAsync(sql, cancellationToken);

    #region Dispose

    private bool _disposed;

    /// <summary>
    /// Disposes of the resources used by the unit of work, including the database context.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronously disposes of the resources used by the unit of work, including the database context.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        await dbContext.DisposeAsync();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            dbContext.Dispose();
        }

        _disposed = true;
    }

    #endregion
}