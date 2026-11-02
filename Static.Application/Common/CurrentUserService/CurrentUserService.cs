using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Static.Domain.Entities;
using Static.EntityFrameworkCore.Repository.Guid;
using Static.Helper.Constants.EnumCollections;

namespace Static.Application.Common.CurrentUserService;

public class CurrentUserService(
    IGenericRepository<User> userRepository,
    IHttpContextAccessor httpContextAccessor
) : UserSession.CurrentUser.Guid.CurrentUserService<StaticRole>(httpContextAccessor), ICurrentUserService
{
    
}

public static class CurrentUserServiceExtensions
{
    public static IServiceCollection AddCurrentUserService(this IServiceCollection services) =>
        services.AddScoped<ICurrentUserService, CurrentUserService>();
}