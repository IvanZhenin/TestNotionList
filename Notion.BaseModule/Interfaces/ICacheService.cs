using System.Collections.Generic;

namespace Notion.BaseModule.Interfaces
{
    public interface ICacheService
    {
        public bool TryGet<T>(string key, out T? value);
        public Task SetAsync<T>(string key, T? value, TimeSpan expiration);
        public Task RemoveAsync(string key);
    }
}