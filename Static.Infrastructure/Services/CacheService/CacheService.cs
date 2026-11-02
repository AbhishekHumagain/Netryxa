using Static.Application.Common.Interfaces;
using Static.Helper.JsonSerializer;
using StackExchange.Redis;

namespace Static.Infrastructure.Services.CacheService;

public class CacheService(IConnectionMultiplexer connectionMultiplexer) : ICacheService
{
    private static readonly TimeSpan DefaultLifeSpan = TimeSpan.FromHours(1);
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    public T? GetData<T>(string key) where T :  class
    {
        var cacheValue =  _database.StringGet(key);
        return cacheValue.IsNullOrEmpty ? null : cacheValue.ToString().Deserialize<T>();
    }
    
    /*public T? GetValueData<T>(string key) where T : IConvertible
    {
        var cacheValue =  _database.StringGet(key);
        return cacheValue.IsNullOrEmpty ? default : (T?) Convert.ChangeType(cacheValue, typeof(T));
    }*/

    public async Task<T?> GetDataAsync<T>(string key) where T : class
    {
        var cacheValue = await _database.StringGetAsync(key);
        return cacheValue.IsNullOrEmpty ? null : cacheValue.ToString().Deserialize<T>();
    }

    public async Task<T> GetDataAsync<T>(string key, Func<Task<T>> factory, TimeSpan? lifeSpan = null) where T : class
    {
        var cacheValue = await GetDataAsync<T>(key);
        if (cacheValue is not null)
        {
            return cacheValue;
        }
        cacheValue = await factory();
        await SetDataAsync(key, cacheValue, lifeSpan ?? DefaultLifeSpan);
        return cacheValue;
    }
    
    public async Task SetDataAsync<T>(string key, T value, TimeSpan? lifeSpan = null) where T : class
    {
        var cacheValue = value.Serialize();
        await SetStringAsync(key, cacheValue , lifeSpan ?? DefaultLifeSpan);
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? lifeSpan = null)
    {
        await _database.StringSetAsync(key, value , lifeSpan ?? DefaultLifeSpan);
    }
    
    public async Task<bool> RemoveDataAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }
    
    public async Task<bool> DoesKeyExistAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }
}