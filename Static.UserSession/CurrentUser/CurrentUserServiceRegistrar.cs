using Microsoft.Extensions.DependencyInjection;
using Static.UserSession.CurrentUser.Service;

namespace Static.UserSession.CurrentUser;

public static class CurrentUserServiceRegistrar
{
    public static IServiceCollection AddBaseCurrentUserService(this IServiceCollection services) =>
        services.AddScoped<ICurrentUserService, CurrentUserService>();

    public static IServiceCollection AddGenericCurrentUserService(this IServiceCollection services)
    {
        services.AddScoped(typeof(ICurrentUserService<>), typeof(CurrentUserService<>));
        services.AddScoped(typeof(ICurrentUserService<,>), typeof(CurrentUserService<,>));
        return services;
    }
}