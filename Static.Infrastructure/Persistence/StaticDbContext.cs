using System.Reflection;
using Static.Application.Common.Interfaces;
using Static.Domain.Entities;
using Static.EntityFrameworkCore.Extensions;
using Static.Infrastructure.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Static.Infrastructure.Persistence.Interceptors;

namespace Static.Infrastructure.Persistence;

public class StaticDbContext(
    DbContextOptions<StaticDbContext> options,
    IMediator? mediator = null,
    AuditingSaveChangesInterceptor? auditingSaveChangesInterceptor = null)
    : DbContext(options), IStaticDbContext

{
    public DbSet<DefaultSetting> DefaultSetting => Set<DefaultSetting>();
    public DbSet<UserNotificationSocket> UserNotificationSocket => Set<UserNotificationSocket>();
    public DbSet<UserNotification> UserNotification => Set<UserNotification>();
    public DbSet<User> User => Set<User>();
    public DbSet<UserLoginAttempt> UserLoginAttempt => Set<UserLoginAttempt>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
        builder.Entity<User>()
               .HasIndex(d => d.EmailAddress)
               .IsUnique();
        builder.Entity<User>()
               .HasIndex(d => d.NickName)
               .IsUnique();

        builder.ApplySoftDeleteQueryFilter();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (auditingSaveChangesInterceptor != null)
        {
            optionsBuilder.AddInterceptors(auditingSaveChangesInterceptor);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (mediator != null)
        {
            await mediator.DispatchDomainEvents(this);
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
