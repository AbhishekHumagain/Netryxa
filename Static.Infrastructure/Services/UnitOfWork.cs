using Static.EntityFrameworkCore.UnitOfWork;
using Static.Infrastructure.Persistence;

namespace Static.Infrastructure.Services;

public class UnitOfWork(StaticDbContext dbContext, IServiceProvider provider) : UnitOfWork<StaticDbContext>(dbContext, provider);
