
using System.Reflection;
using FluentValidation;
using Static.Application.Common.CurrentUserService;
using Static.MediatR.Behaviours.AntiXss;
using Static.MediatR.Behaviours.Cache;
using Static.MediatR.Behaviours.Cache.KeyGenerator;
using Static.MediatR.Behaviours.Logging;
using Static.MediatR.Behaviours.Tracing;
using Static.MediatR.Behaviours.Performance;
using Static.MediatR.Behaviours.UnhandledException;
using Static.MediatR.Behaviours.Validation;
using Static.QueryUtils.Pagination.Validators;
using Static.UserSession.CurrentUser;
using Static.UserSession.CurrentUser.Guid;
using Microsoft.Extensions.DependencyInjection;
using Static.Application.Common.Behaviours;
using Static.Application.Common.UserSync;

namespace Static.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(TracingBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
            cfg.AddOpenBehavior(typeof(CachingBehaviour<,>));
            cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });

        services.AddPaginatedQueryValidators();
        
        services.AddCurrentUserService();
        services.AddBaseCurrentUserService();
        services.AddGuidBasedCurrentUserService();
        
        services.AddDistributedMemoryCache();
        services.AddCacheKeyGenerator();

        services.AddUserSyncService();

        return services;
    }
}