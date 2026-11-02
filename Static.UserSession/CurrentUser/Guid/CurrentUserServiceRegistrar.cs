using Static.Helper.Constants.EnumCollections;
using Microsoft.Extensions.DependencyInjection;

namespace Static.UserSession.CurrentUser.Guid;

public static class CurrentUserServiceRegistrar
{
    public static IServiceCollection AddGuidBasedCurrentUserService(this IServiceCollection services)
    {
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        services.AddTransient<ICurrentUserService<StaticRole>, CurrentUserService<StaticRole>>();
        return services;
    }
}