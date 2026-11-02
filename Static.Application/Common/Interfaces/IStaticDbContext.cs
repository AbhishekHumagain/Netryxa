namespace Static.Application.Common.Interfaces;

public interface IStaticDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
    
}
