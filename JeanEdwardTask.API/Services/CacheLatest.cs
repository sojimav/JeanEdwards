using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace JeanEdwardTask.API.Services
{
    public class CacheLatest
    {
        private readonly IMemoryCache _cache;

        public CacheLatest(IMemoryCache cache)
        {
            _cache = cache;
        }

        private string LatestSearchesKey { get; set; } = "LatestSearches";

        // Save latest searches
        public void SaveSearchQuery(string query)
        {
            if (_cache.TryGetValue(LatestSearchesKey, out List<string> latestSearches))
            {
                if (!latestSearches.Contains(query))
                {
                    latestSearches.Insert(0, query);
                    if (latestSearches.Count > 5) latestSearches.RemoveAt(5);
                }
            }
            else
            {
                latestSearches = new List<string> { query };
            }

            _cache.Set(LatestSearchesKey, latestSearches);
        }


        public List<string> GetLatestSearches()
        {
            if (_cache.TryGetValue(LatestSearchesKey, out List<string> latestSearches))
            {
                return latestSearches;
            }

            return new List<string>();
        }
    }
}
