namespace Static.MediatR.Behaviours.Cache.KeyGenerator;

public interface ICacheKeyGenerator
{
    string GenerateRequestBasedCacheKey(bool includeUserId = true);
    string GenerateRequestBasedCacheKeyFrom(string key, bool includeUserId = true);

}