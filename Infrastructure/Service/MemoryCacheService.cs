using Microsoft.Extensions.Caching.Memory;
using StudentMgmt.Application.Interfaces;

namespace StudentMgmt.Infrastructure.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        public const string DashboardCacheKey = "student_dashboard_stats";

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }
        public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            var result = await _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10);
                return await factory();
            });

            return result;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void RemoveDashboardCache()
        {
            _cache.Remove(DashboardCacheKey);
        }
    }
}