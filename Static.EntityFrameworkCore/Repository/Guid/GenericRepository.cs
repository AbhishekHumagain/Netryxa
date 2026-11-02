using Microsoft.EntityFrameworkCore;
using Static.EntityFrameworkCore.Contracts.Entity;

namespace Static.EntityFrameworkCore.Repository.Guid;

public class GenericRepository<TEntity, TDbContext>(TDbContext context) : GenericRepository<TEntity, System.Guid, TDbContext>(context), IGenericRepository<TEntity> where TEntity : class, IEntity<System.Guid>
    where TDbContext : DbContext;