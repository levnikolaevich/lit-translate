using SDT.Bl.IHelpers;

namespace SDT.Bl.Helpers
{
    public class GlobalCacheService<T> : IGlobalCacheService<T> where T : new()
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
        private CachedData<T>? _cachedPriceListResponse;

        public T GetPriceList()
        {
            if (_cachedPriceListResponse != null && (DateTime.UtcNow - _cachedPriceListResponse.CacheTime) < CacheDuration)
            {
                return _cachedPriceListResponse.Data;
            }
            return new T();
        }

        public void SetPriceList(T response)
        {
            _cachedPriceListResponse = new CachedData<T>
            {
                Data = response,
                CacheTime = DateTime.UtcNow
            };
        }
    }
    public class CachedData<T>
    {
        public required T Data { get; set; }
        public DateTime CacheTime { get; set; }
    }
}
