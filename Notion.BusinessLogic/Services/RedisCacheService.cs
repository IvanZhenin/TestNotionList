using Notion.BaseModule.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Notion.BusinessLogic.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _redisDb;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _redisDb = _redis.GetDatabase();
        }

        public async Task RemoveAsync(string key)
        {
            await _redisDb.KeyDeleteAsync(key);
        }

        public async Task SetAsync<T>(string key, T? value, TimeSpan expiration)
        {
            var json = JsonSerializer.Serialize(value);
            await _redisDb.StringSetAsync(key, json, expiration);
        }

        public bool TryGet<T>(string key, out T? value)
        {
            var redisValue = _redisDb.StringGet(key);
            if (!redisValue.HasValue)
            {
                value = default;
                return false;
            }

            value = JsonSerializer.Deserialize<T>(redisValue!);
            return true;
        }
    }
}