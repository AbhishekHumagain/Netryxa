using Static.EntityFrameworkCore.Contracts.Entity;
using Static.EntityFrameworkCore.Repository.Guid;
using Static.Infrastructure.Persistence;

namespace Static.Infrastructure.Services;

public class GenericRepository<T>(StaticDbContext dbContext) : GenericRepository<T, StaticDbContext>(dbContext) where T : class, IEntity;

