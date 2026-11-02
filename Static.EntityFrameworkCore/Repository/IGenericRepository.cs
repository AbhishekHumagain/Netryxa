using Microsoft.EntityFrameworkCore.Query;
using Static.EntityFrameworkCore.Contracts.Entity;
using System.Linq.Expressions;

namespace Static.EntityFrameworkCore.Repository
{
    /// <summary>
    /// Generic repository interface for CRUD operations on entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the entity's primary key.</typeparam>
    public interface IGenericRepository<TEntity, TPrimaryKey>
        where TPrimaryKey : struct, IEquatable<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        #region Select/Get/Query

        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllNoTracking();

        Task<(IEnumerable<TProjectedType> Items, int TotalCount)> GetAllAsync<TProjectedType>(
            (int Page, int Size) pagination,
            bool calculateTotalCount = true,
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>[]? includes = null,
            (bool IsAscending, Expression<Func<TEntity, object>> KeySelector)[]? orderBy = null,
            Expression<Func<TEntity, TProjectedType>>? selector = null,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<TProjectedType>> GetAllAsync<TProjectedType>(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>[]? includes = null,
            (bool IsAscending, Expression<Func<TEntity, object>> KeySelector)[]? orderBy = null,
            Expression<Func<TEntity, TProjectedType>>? selector = null,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default);

        Task<TProjectedType?> GetAsync<TProjectedType>(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>[]? includes = null,
            Expression<Func<TEntity, TProjectedType>>? selector = null,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default);

        Task<TEntity?> GetAsync(TPrimaryKey id,
            Expression<Func<TEntity, object>>[]? includes = null,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default);

        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default);

        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(TPrimaryKey id, CancellationToken cancellationToken = default);

        #endregion

        #region Insert

        void Insert(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default);
        void InsertRange(IEnumerable<TEntity> entities);
        Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        Task BulkInsertAsync(IEnumerable<TEntity> entities,
            int batchSize = 100,
            CancellationToken cancellationToken = default);

        #endregion

        #region Update

        void Update(TEntity entity);

        Task<int> UpdateAsync(
            Action<UpdateSettersBuilder<TEntity>> setPropertyCalls,
            CancellationToken cancellationToken = default);

        Task<int> UpdateAsync(
            Expression<Func<TEntity, bool>> condition,
            Action<UpdateSettersBuilder<TEntity>> setPropertyCalls,
            CancellationToken cancellationToken = default);

        void UpdateRange(IEnumerable<TEntity> entities);

        Task BulkUpdateAsync(IEnumerable<TEntity> entities,
            int batchSize = 100,
            CancellationToken cancellationToken = default);

        #endregion

        #region Delete

        void Delete(TEntity entity);
        Task HardDeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default);
        Task HardDeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task HardDeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        void DeleteRange(IEnumerable<TEntity> entities);
        Task HardDeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task HardDeleteRangeAsync(IEnumerable<TPrimaryKey> ids, CancellationToken cancellationToken = default);

        #endregion

        #region Aggregates

        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<long> LongCountAsync(CancellationToken cancellationToken = default);

        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        #endregion

        #region Raw SQL

        IQueryable<TEntity> FromSql(FormattableString sql);
        IQueryable<TEntity> FromSql(string sql, object[] parameters);
        Task<IEnumerable<TEntity>> FromSqlAsync(FormattableString sql, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> FromSqlAsync(string sql,
            object[] parameters,
            CancellationToken cancellationToken = default);

        #endregion
    }
}