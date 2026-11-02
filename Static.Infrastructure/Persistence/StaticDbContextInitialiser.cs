using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Static.Infrastructure.Persistence;

public class StaticDbContextInitializer
{
    private readonly ILogger<StaticDbContextInitializer> _logger;
    private readonly StaticDbContext _context;

    public StaticDbContextInitializer(ILogger<StaticDbContextInitializer> logger, StaticDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
}
