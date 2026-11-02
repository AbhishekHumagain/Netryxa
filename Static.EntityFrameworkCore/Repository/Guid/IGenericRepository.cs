using Static.EntityFrameworkCore.Contracts.Entity;

namespace Static.EntityFrameworkCore.Repository.Guid;

/// <summary>
/// Generic repository interface for CRUD operations on entities, with a default primary key type of <see cref="Guid"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IGenericRepository<TEntity> : IGenericRepository<TEntity, System.Guid>
    where TEntity : class, IEntity<System.Guid>;