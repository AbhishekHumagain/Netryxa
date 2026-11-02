using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Static.EntityFrameworkCore.Contracts.Entity;
using Static.EntityFrameworkCore.Repository;
using IsolationLevel = System.Data.IsolationLevel;

namespace Static.EntityFrameworkCore.UnitOfWork;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Commits all changes made within the unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation, with the number of state entries written to the database.</returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a database transaction with the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">The isolation level for the transaction. Defaults to <see cref="IsolationLevel.Unspecified"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation, with the transaction object.</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default);

    
    /// Only use for entities without <see cref="IEntity{TPrimaryKey}"/> 
    DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class;
    
    /// <summary>
    /// Gets a generic repository for managing entities of the specified type with a custom primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
    /// <returns>An instance of <see cref="IGenericRepository{TEntity,TPrimaryKey}"/>.</returns>
    IGenericRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>()
        where TPrimaryKey : struct, IEquatable<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>;

    /// <summary>
    /// Gets a generic repository for managing entities of the specified type with a <see cref="Guid"/> primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>An instance of <see cref="Repository.Guid.IGenericRepository{TEntity}"/>.</returns>
    Repository.Guid.IGenericRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class, IEntity<Guid>;

    /// <summary>
    /// Executes a raw SQL query and returns a queryable result set.
    /// </summary>
    /// <typeparam name="TResult">The type of the result set.</typeparam>
    /// <param name="sql">The raw SQL query to execute.</param>
    /// <param name="parameters">An array of parameters to pass to the query.</param>
    /// <returns>An <see cref="IQueryable{T}"/> representing the result set.</returns>
    IQueryable<TResult> FromSql<TResult>(string sql, object[] parameters);

    /// <summary>
    /// Executes a formatted SQL query and returns a queryable result set.
    /// </summary>
    /// <typeparam name="TResult">The type of the result set.</typeparam>
    /// <param name="sql">The formatted SQL query to execute.</param>
    /// <returns>An <see cref="IQueryable{T}"/> representing the result set.</returns>
    IQueryable<TResult> FromSql<TResult>(FormattableString sql);

    /// <summary>
    /// Executes a raw SQL command asynchronously.
    /// </summary>
    /// <param name="sql">The raw SQL command to execute.</param>
    /// <param name="parameters">An array of parameters to pass to the command.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation, with the number of rows affected.</returns>
    Task<int> ExecuteSqlAsync(string sql, object[] parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a raw SQL command asynchronously.
    /// </summary>
    /// <param name="sql">The raw SQL command to execute.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation, with the number of rows affected.</returns>
    Task<int> ExecuteSqlAsync(FormattableString sql, CancellationToken cancellationToken = default);
}