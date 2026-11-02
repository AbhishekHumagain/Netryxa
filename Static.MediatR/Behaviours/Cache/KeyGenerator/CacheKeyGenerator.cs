using Static.UserSession.CurrentUser.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Static.MediatR.Behaviours.Cache.KeyGenerator;

public class CacheKeyGenerator(ICurrentUserService currentUserService) : ICacheKeyGenerator
{
    public string GenerateRequestBasedCacheKey(bool includeUserId = true)
    {
        var queryString = currentUserService.HttpContext.Request.QueryString;
        var path = currentUserService.HttpContext.Request.Path.ToString();
        var requestPathWithQuery = queryString.HasValue
            ? path + ":" + queryString
            : path;

        var cacheKey = currentUserService.IsUserAuthenticated && includeUserId
            ? currentUserService.UserId + ":" + requestPathWithQuery
            : requestPathWithQuery;
        return cacheKey;
    }

    public string GenerateRequestBasedCacheKeyFrom(string key, bool includeUserId = true) => GenerateRequestBasedCacheKey(includeUserId) + ":" + key;
}

public static class CacheKeyGeneratorExtensions
{
    public static IServiceCollection AddCacheKeyGenerator(this IServiceCollection services)
        => services.AddScoped<ICacheKeyGenerator, CacheKeyGenerator>();
}